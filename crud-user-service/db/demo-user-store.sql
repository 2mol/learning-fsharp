---------------------------------------------------------------------------------
-- 0. Setup
---------------------------------------------------------------------------------

create extension "uuid-ossp";

--drop table users;

create table users (
	id uuid default uuid_generate_v4() primary key not null,
	data jsonb not null,
	created_at timestamptz default now() not null
);

---------------------------------------------------------------------------------
-- 1. Create some data
---------------------------------------------------------------------------------

begin;
delete from users;

insert into
  users (data)
select
  '
  { "name": "Juri Chomé"
  , "email": null
  , "email_verified": true
  , "birthdate": "1986-09-02"
  , "favourite_number": null
  , "addresses": [{"street": "Manessestrasse", "number": "132", "postcode": 8045, "city": "Zürich"}]
  }
  '
from generate_series(1,100000) i;

update users
set data=jsonb_set(data, '{favourite_number}', to_jsonb((random() * 100)::int));

update users
set data=jsonb_set(data, '{email}', to_jsonb(uuid_generate_v4()::text || '@fastmail.com'));

commit;

---------------------------------------------------------------------------------
-- 2. Let's have a look
---------------------------------------------------------------------------------

select * from users limit 10;


-- How big is that data?

vacuum full users;
select pg_size_pretty(pg_database_size('dojo'));


-- How do I even query JSON?
-- https://www.postgresql.org/docs/12/functions-json.html

select * from users
where data ->> 'favourite_number' = '42';

select id, data -> 'addresses' -> 0 ->> 'city' as city from users;

select
	  data ->> 'name' as name
	, (data ->> 'birthdate') :: date as birthdate
	, (data ->> 'favourite_number') :: int as favourite_number
from users
limit 10;


select users.id, rec.* from users, jsonb_to_record(data)
as
rec(
  name text
, birthdate date
, favourite_number int
)
limit 10
;

---------------------------------------------------------------------------------
-- 3. But it doesn't have a schema or any guarantees!
---------------------------------------------------------------------------------

insert into users (data)
values ('{"birthdate": "2000-01-01", "name": ""}');

--delete from users where data ->> 'name' != 'Juri Chomé';


alter table users
drop constraint data_constraints;


-- we could also reuse these constraints with CREATE DOMAIN
alter table users
add constraint data_constraints
check (
  (data->>'name') is not null
  and
  length(data->>'name') > 3
  and
  (data->>'birthdate') :: date is not null
  and
  (data->>'gender') in ('radium', 'war', 'ponies')
  and
  jsonb_typeof(data->'addresses') = 'array'
  and
  data ->> 'email' ~ '^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+[.][A-Za-z]+$'

-- ...
--  and
--  (data ->> 'username' ~ '^[a-zA-Z0-9]+$')
--  and
--  ((data->>'gender')::gender) is not null
--  and
--  jsonb_typeof(data->'favourite_number') = 'number'
--  and
--  (data->>'favourite_number')::bigint > 0
--  and
--  length(((doc->>'favourite_number')::bigint)::text) = 13
);

--create type gender as enum ('radium', 'war', 'ponies');

insert into users (data)
values ('{"birthdate": "2000-01-01", "name": "John", "gender": "war"}')
returning id
;

insert into users (data)
values ('{"birthdate": "2000-01-01", "name": "John", "gender": "radium", "email": "& bobby tables ^&"}')
returning id
;

update users
set data = '{"birthdate": "2000-01-01", "name": "John", "gender": "radium", "email": "nah"}'
where id = 'ff805e49-2ef5-45a5-afdb-f2e319c9b37b';

select * from users where id = 'ff805e49-2ef5-45a5-afdb-f2e319c9b37b';


select (data ->> 'gender') :: gender from users
where data ->> 'gender' is not null;


---------------------------------------------------------------------------------
-- 4. Other
---------------------------------------------------------------------------------

create unique index users_lower_email_idx
on users(
  lower(data->>'email')
);








---------------------------------------------------------------------------------
-- Notes
---------------------------------------------------------------------------------

--    jsonb_typeof(j_string)   = 'string'  and
--    jsonb_typeof(j_number)   = 'number'  and
--    jsonb_typeof(j_boolean)  = 'boolean' and
--    jsonb_typeof(j_null)     = 'null'    and
--    jsonb_typeof(j_object)   = 'object'  and
--    jsonb_typeof(j_array)    = 'array',
