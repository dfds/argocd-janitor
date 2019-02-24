-- 2019-02-24 22:08:57 : create_capability_table
CREATE TABLE public."Capability" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "AzureADObjectId" varchar(255) NULL,
    CONSTRAINT capability_pk PRIMARY KEY ("Id")
);