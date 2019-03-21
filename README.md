[![Build Status](https://dfds.visualstudio.com/DevelopmentExcellence/_apis/build/status/Blaster-CI?branch=master)](https://dfds.visualstudio.com/DevelopmentExcellence/_build/latest?definitionId=803&branch=master)[![Release Status](https://dfds.vsrm.visualstudio.com/_apis/public/Release/badge/ace5e409-c242-4356-93f4-23c53a3dc87b/14/18)](https://dfds.visualstudio.com/DevelopmentExcellence/_build/latest?definitionId=803&branch=master)

# argocd-janitor

Janitor for integrating *blaster* with ArgoCD, a Contiunous Delivery platform. 
The argocd-janitor creates projects in ArgocD and sets up the required configuration, every time a *capability* is created in *blaster*.

# architecture

The janitor listens to a Kafka Topic named *build.capabilities*, and handles the *capability_created*-event. 
Whenever an event is registered, the janitor tries to create a matching *project* in ArgoCD using ArgoCD REST API.


```ascii
                             
                             
+---------------------+      +--------------------+     +------------------+     +---------------+
|                     |      |       Kafka        |     |                  |     |               |
| capability_created  +----->+ build.capabilities +---->+  argocd-janitor  +---->+ ArgoCD Server |
|                     |      |                    |     |                  |     |               |
+---------------------+      +--------------------+     +------------------+     +---------------+
```

# dependencies
    [ArgoCD](https://github.com/argoproj/argo-cd)