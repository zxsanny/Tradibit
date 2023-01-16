# Tradibit
## Cryptocurrency Automated Trading system

### Dependencies
* .net 6 or higher supported IDE (Rider or Visual Studio)
* postgresql

### Installation
1. Add environment variables with some random data for encryption/decryption:
```shell
TradibitAesKey
TradibitAesIv
```

2. Add psql's folder to env vars, and run initial.sql for create db and user. But use 'grant all' only for dev's dbs:
```shell
psql -U postgres -f env/initial.sql
```


### License
Copyright (c) 2022, Oleksandr Bezdieniezhnykh
