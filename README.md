# Tradibit
## Cryptocurrency Automated Trading system

### Dependencies
* .net 6 or higher supported IDE (Rider or Visual Studio)
* postgresql

### Installation
1. Environment configuration:
 - Add environment variables with some random data for encryption/decryption: 
  ```shell 
  TradibitAesKey 
  TradibitAesIv 
  ```
2. Database:
 - Add psql's folder to PATH, and run initial.sql for create db and user. But use 'grant all' only for dev's dbs:
    ```shell
    psql -U postgres -f env/initial.sql
    ```
 - Update database with migrations (Also make sure your dotnet folder and dotnet tools folder is in the PATH):
   ```shell
    cd Tradibit.Api
    dotnet tool install --global dotnet-ef
    dotnet ef database update
    ```

### License
Copyright (c) 2022, Oleksandr Bezdieniezhnykh
