SELECT 'CREATE DATABASE "nivadUserDB"'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'nivadUserDB')\gexec

SELECT 'CREATE DATABASE inventory_service'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'inventory_service')\gexec

