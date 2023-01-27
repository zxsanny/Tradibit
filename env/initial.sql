-- 1. Change password. 2. Granting all privileges should be only on development database
create database tradibit;
create user tradibit with encrypted password 'PaWeHPtwfL';
\c tradibit
grant all privileges on database tradibit to tradibit;
grant all on schema public to tradibit;
