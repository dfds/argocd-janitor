#!/bin/bash

set -eu

GITREPO=$1
TAG=$2


# Exit if variable is empty
if [ -z "$GITREPO" ]
then
    echo "\$GITREPO is empty"
    exit 1
fi

# Exit if variable is empty
if [ -z "$TAG" ]
then
    echo "\$TAG is empty"
    exit 1
fi

mkdir gitops
cd gitops

git clone $GITREPO .

ls -la

cd selfservice/overlays/production

kustomize edit set image 579478677147.dkr.ecr.eu-central-1.amazonaws.com/ded/argojanitor:$TAG
git add . -m "Set image tag to $TAG"

# Rebase in case we have a race condition
git pull --rebase 
git push

#kustomize edit ..

#git add .

#git commit ..


## ---
#git add .

#git commit --message "$MESSAGE"

#git pull --rebase

#git push