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
