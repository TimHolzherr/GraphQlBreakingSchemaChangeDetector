# GraphQl Breaking Schema Change Detector
.Net tool which can detect breaking schema changes in a GraphQl schema.  
Includes support for automatic comments creation in Azure Devops Pr pipelines.

![BreakingChangeDetectorPrComment](https://user-images.githubusercontent.com/11144100/156728254-8897d42b-3440-44f4-aed5-63daa65dad0f.png)

## Why
When maintaining a GraphQl api the convention is to not version the graph but instead extend it in a backwards compatibel way.  
The possibility to solve api versioning this way is one of the great strenghts of GraphQl but one needs to ensure that the graph is indeed always backwards compatibel.
As the name implies the 'GraphQl Breaking Schema Change Detector' allows for automatically detecting such breaking changes. 
It can either compare two GraphQl schemas and report breaking changes or it allows to validate the schema during pull request stage.



## How to compare two GraphQl Schemas via Cli
1. Make sure you have installed the [dotnet sdk](https://dotnet.microsoft.com/en-us/download)
1. Install the Breaking Change Detector as a global dotnet tool: `dotnet tool install -g graphql-breaking-schema-change-detector`
1. Compare two schemas with the following command  
 ```breaking-change file --oldSchema <old schema file> --newSchema <new schema file>```

## Setup Azure Devops PR Validation

The azure devops integration allows you to monitor a graphql schema file for backward incompatibel changes.
This is valuable when your schema is automatically generated based on your source code.
If one of your unit test ensures that the schema of your GraphQl service is corresponding to a GraphQl schema file under source control then the GraphqlBreakingSchemaChangeDetector can monitor this file for breaking changes (see [example](https://github.com/TimHolzherr/GraphQlBreakingSchemaChangeDetector/tree/main/examples/CodeFirstHotChocolate)).

To enable Azure DevOps integration 4 steps are needed.

1. Create a new yaml file containing the definition of a new PR validation build
2. Create a new pipeline based on the new yaml file.
3. Add the new pipeline as a PR Build validation step
4. Give the Build Agent the needed permissions to create comments on the pull request

### Create a new yaml file 
Create a new yaml file in your source repository with the following content.
```yml
pr:
  - main

pool:
  vmImage: ubuntu-latest

steps:
  - task: UseDotNet@2
    inputs:
      version: "6.0.x"
      includePreviewVersions: false
  - script: |
      dotnet tool install --tool-path . graphql-breaking-schema-change-detector
      ./breaking-change pr --file <PATH TO YOUR SCHEMA FILE>
    env:
      SYSTEM_ACCESSTOKEN: $(System.AccessToken)
    displayName: "Run Breaking Change Detector"
```
Make sure to replace `<PATH TO YOUR SCHEMA FILE>` with the file path to the schema file you want to monitor. 
The file path must be relative to the root of your repository. Example: `test/foo/bar/schema.graphql`.


### Create a new Pipeline.
![NewPipeline_redboxes](https://user-images.githubusercontent.com/11144100/156795002-5121c270-557b-4b96-9e70-b9e6b95b701d.png)
  
1. Choose your source provider and connect to your repository
2. In "Configure your pipeline" choose the "Existing Azure Pipelines YAML file" and select the azure-pipelines.yml file
3. Save your new pipeline

### Add the new pipeline as a PR Build Validation step
![OpenBranchPolicy_redboxes](https://user-images.githubusercontent.com/11144100/156795141-a8b3b244-498e-4f3f-b628-3ed745159a45.png)

1. Go to your repository
2. Choose Repos -> Branches -> Press the three dots on your main branch -> Branch policies
3. Build Validation -> Plus Sign -> Choose your new pipeline from the "Build pipeline" dropdown
4. Save

![BuildValidation_redBoxe](https://user-images.githubusercontent.com/11144100/156795179-6a448bc4-9989-4107-843b-f295032c12cd.png)


### Allow the Build Agent to "Contribute to pull requests"
![ProjectSettings_GiveBuildAgendAccess_redboxes](https://user-images.githubusercontent.com/11144100/156795271-a39bc8c2-d93a-4f31-b349-b0d878db7ea7.png)
  
1. Go to Project settings (bottom left)
2. Repositories -> Choose your repository -> Security Tab -> Users -> Build Agend -> Contribute to pull requests -> Set to Allow
