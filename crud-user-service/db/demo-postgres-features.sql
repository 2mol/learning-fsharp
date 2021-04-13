---------------------------------------------------------------------------------
-- Generated columns
---------------------------------------------------------------------------------

drop table temperature_log;
create table temperature_log (
	temperature_metric numeric(12,2),
	temperature_fahrenheit numeric(12,2) generated always as ((temperature_metric * 9/5) + 32) stored,
	measured_at timestamptz default now() not null
);

insert into temperature_log (temperature_metric) values (0), (30), (60), (180), (220);

select * from temperature_log;


---------------------------------------------------------------------------------
-- Enums
---------------------------------------------------------------------------------

create type medium as enum ('book', 'essay', 'article', 'podcast', 'videogame');

drop table my_favourite_things;
create table my_favourite_things (
	title text not null,
	medium medium,
	thoughts text
);

insert into my_favourite_things (title, medium, thoughts)
values ('Disco Elysium', 'videogame', 'Excellent game, but anarcho-capitalism is under-represented.');

select * from my_favourite_things;


---------------------------------------------------------------------------------
-- Constraints
---------------------------------------------------------------------------------

-- (will show in user-manager)


---------------------------------------------------------------------------------
-- JSON
---------------------------------------------------------------------------------

-- (will show in user-manager)


---------------------------------------------------------------------------------
-- Triggers
---------------------------------------------------------------------------------

create or replace function trigger_set_timestamp()
returns trigger as $$
begin
  new.updated_at = now();
  return new;
end;
$$ language plpgsql;


drop table user_names;
create table user_names (
  name text not null,
  created_at timestamp with time zone default now() not null,
  updated_at timestamp with time zone default now() not null
);

create trigger set_timestamp_user_names
  before update on user_names
  for each row
  execute function trigger_set_timestamp();

insert into user_names values ('Juri');

update user_names set name = 'Juri Dojo';
select * from user_names;


---------------------------------------------------------------------------------
-- (Materialized) views
---------------------------------------------------------------------------------

-- (will show over there in the dvd-rental)


---------------------------------------------------------------------------------
-- Row-level security
---------------------------------------------------------------------------------

-- will show you some other time


---------------------------------------------------------------------------------
-- Random other stuff
---------------------------------------------------------------------------------

-- brief excursion into postgREST?

--CREATE EXTENSION pg_stat_statements;
--SELECT * FROM pg_stat_statements;


-- https://begriffs.com/posts/2017-10-21-sql-domain-integrity.html