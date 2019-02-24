#!/bin/sh
#
# Run the Postgres database migrations as well as seeding the database

readonly GREEN="\\033[32m"
readonly RESET="\\033[0m"
readonly PSQL="psql -X -q -v ON_ERROR_STOP=1 --pset pager=off"
readonly MIGRATION_TABLE_NAME="_migrations"

#export GOPTIONS='--client-min-messages=warning'

if [ -n "${DEBUG+set}" ]; then
    set -x
fi

set -eu

##############################################################################
# wait for database to come online
# Globals:
#   PGHOST
#   PGPORT
# Arguments:
#   None
# Returns:
#   None
##############################################################################
wait_for_database() {
    readonly TIMEOUT=1s
    until pg_isready -q -h ${PGHOST} -p ${PGPORT}; do
        echo "postgres is unavailable - waiting ${TIMEOUT}..."
        sleep $TIMEOUT
    done

    echo "postgres is up - preparing database migrations"
}

##############################################################################
# Create database (dropping if needed) for local development
# Globals:
#   LOCAL_DEVELOPMENT
#   PGDATABASE
#   PSQL
#   GREEN
#   RESET
# Arguments:
#   None
# Returns:
#   None
##############################################################################
create_database() {
    if [ -n "${LOCAL_DEVELOPMENT+set}" ]; then
        echo -e "Creating database ${GREEN}${PGDATABASE}${RESET}..."
        ${PSQL} -d postgres -c "DROP DATABASE IF EXISTS ${PGDATABASE};"
        ${PSQL} -d postgres -c "CREATE DATABASE ${PGDATABASE};"
    fi
}

##############################################################################
# Create migration table if needed
# Globals:
#   None
# Arguments:
#   None
# Returns:
#   None
##############################################################################
create_migration_table() {
    echo -e "Creating ${GREEN}_migration${RESET} table..."
    ${PSQL} -1 <<SQL
CREATE TABLE IF NOT EXISTS "${MIGRATION_TABLE_NAME}"
(
    script_file  varchar(255) NOT NULL PRIMARY KEY,
    hash         varchar(64) NOT NULL,
    date_applied timestamp NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "${MIGRATION_TABLE_NAME}_script_file_idx" ON "${MIGRATION_TABLE_NAME}" (script_file);
SQL
}

##############################################################################
# Run migrations/*.sql scripts
# Globals:
#   GREEN
#   RESET
# Arguments:
#   None
# Returns:
#   None
##############################################################################
run_migrations() {
    if ls migrations/*.sql 1> /dev/null; then
        echo "Preparing migration script..."

        migration_script="/tmp/script.sql"

        cat << EOF > ${migration_script}
/* Script was generated on $(date '+%Y-%m-%d %H:%M:%S') */

LOCK TABLE ONLY "${MIGRATION_TABLE_NAME}" IN ACCESS EXCLUSIVE MODE;

--SELECT pg_sleep(60);

EOF

        for entry in $(ls migrations/*.sql | sort)
        do
            echo -e "Adding migration ${GREEN}${entry}${RESET}"
        
            name=$(basename $entry)
            hash=$(sha1sum $entry | cut -f 1 -d ' ')

            cat << EOF >> ${migration_script}

--
-- BEG: ${name}
--
DO \$\$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ${MIGRATION_TABLE_NAME} WHERE script_file = '${name}') THEN
EOF
            cat $entry | sed -e 's,^,\t,g' >> ${migration_script}

            cat <<EOF >> ${migration_script}

        INSERT INTO "${MIGRATION_TABLE_NAME}" (script_file, hash, date_applied) VALUES ('${name}', '${hash}', NOW());
    END IF;
END;
\$\$;
--
-- END: $name
--
EOF
        done

        echo "Running migration script..."
        ${PSQL} -1 -f ${migration_script}
    fi
}

##############################################################################
# seed local database table if needed
# Globals:
#   LOCAL_DEVELOPMENT
#   GREEN
#   RESET
# Arguments:
#   None
# Returns:
#   None
##############################################################################
seed_database() {
    readonly ORDER_FILE="seed/_order"

    if [ -n "${LOCAL_DEVELOPMENT+set}" ] && [ -f ${ORDER_FILE} ]; then
        echo "Importing seed data..."

        base_path=$(dirname $(readlink -f "$0"))

        while read name; do
            table_name=$(echo $name | cut -f 1 -d '.')
            file="$base_path/seed/$name"

            echo -e "Seeding ${GREEN}${table_name}${RESET} with ${GREEN}${file}${RESET}"
            ${PSQL} -1 -c "\\copy \"${table_name}\" FROM '$file' WITH DELIMITER ',' CSV HEADER;"
        done < ${ORDER_FILE}
    fi
}

wait_for_database
create_database
create_migration_table
run_migrations
seed_database

echo "Done"

