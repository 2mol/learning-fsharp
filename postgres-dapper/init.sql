create extension "uuid-ossp";

create table person (
    id uuid primary key,
    first_name text not null,
    last_name text not null,
    position int not null,
    date_of_birth date null
);
