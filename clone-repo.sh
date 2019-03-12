#!/bin/bash

set -eu

GITREPO=$1


# Exit if variable is empty
if [ -z "$GITREPO" ]
then
    echo "\$GITREPO is empty"
    exit 1
fi

mkdir gitops
cd gitops

git clone $GITREPO

#kustomize edit ..

#git add .

#git commit ..


## ---
#git add .

#git commit --message "$MESSAGE"

#git pull --rebase

#git push