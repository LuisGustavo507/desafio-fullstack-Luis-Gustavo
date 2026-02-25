#!/bin/bash
set -e

echo "Iniciando PostgreSQL..."
service postgresql start

echo "Definindo senha do postgres (modo peer)..."
su postgres -c "psql -c \"ALTER USER postgres WITH PASSWORD 'postgres';\""

echo "Alterando autenticação para md5..."
sed -i "s/peer/md5/g" /etc/postgresql/*/main/pg_hba.conf

echo "Reiniciando PostgreSQL..."
service postgresql restart

echo "Criando banco webclima..."
export PGPASSWORD=postgres
psql -U postgres -h localhost -c "CREATE DATABASE webclima;" || true

echo "Iniciando aplicação..."
dotnet WebClima.dll