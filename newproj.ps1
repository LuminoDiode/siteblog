dotnet new globaljson;
dotnet new WebApi -o backend;
dotnet new sln --name siteblog;
dotnet sln add backend/backend.csproj;
dotnet new gitignore;
npx create-react-app client --template typescript;
New-Item image_indexator_backend/dockerfile;
New-Item image_indexator_client/dockerfile;
New-Item docker-compose.yml;
New-Item docker-compose.override.yml;
New-Item .dockerignore;
Set-Content .dockerignore '**/.classpath
**/.dockerignore
**/.env
**/.git
**/.gitignore
**/.project
**/.settings
**/.toolstarget
**/.vs
**/.vscode
**/*.*proj.user
**/*.dbmdl
**/*.jfm
**/azds.yaml
**/bin
**/charts
**/docker-compose*
**/Dockerfile*
**/node_modules
**/npm-debug.log
**/obj
**/secrets.dev.yaml
**/values.dev.yaml
LICENSE
README.md'
git init;