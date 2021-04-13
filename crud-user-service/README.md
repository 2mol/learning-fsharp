# User Service CRUD API

This service proof-of-concept uses a simple PostgreSQL `jsonb` column to store user data.

The rationale for why this is robust is as follows: For a single service that has complete ownership over the database, we can assume that a JSON saved in the database has passed through a strongly typed F# value.

The following illustration visualized this:

![](docs/api.svg)

For a lot of use cases, we truly get the best of both worlds:

- Strong guarantees that the serialized JSON value conforms to a schema.
- The F# types that model our domain very precisely.
- The time-saver of not _also_ having to design a database relational model and schemas.
- The flexibility to extend the data model.
- Optional constraints on the database in case we want to be extra sure that certain fields are not null, conform to a JSON type, or represent a valid email address/phone number, etc.

For optional examples on how to work with JSON in Postgres (indexing on a field, creating constraints, mapping back to a tabular shape, ...) you can have a peek at [db/demo-user-store.sql](db/demo-user-store.sql).


## Setup

In order to run this, a Postgres instance has to be listening to port 5432. The first time, a new database has to be created by executing `create database userservice;` in psql or similar.

You can then initialize the table + example data with

```
psql -f db/init.sql
```
