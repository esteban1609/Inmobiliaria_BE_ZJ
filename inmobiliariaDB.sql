
CREATE DATABASE IF NOT EXISTS reservas_temporales;
USE reservas_temporales;

DROP TABLE IF EXISTS propietario;
DROP TABLE IF EXISTS inquilino;

-- =========================================================
-- Creación de tablas
-- =========================================================

CREATE TABLE propietario (
    idpropietario   INT AUTO_INCREMENT PRIMARY KEY,
    dni              VARCHAR(15) NOT NULL UNIQUE,
    nombre           VARCHAR(100) NOT NULL,
    apellido         VARCHAR(100) NOT NULL,
    telefono         VARCHAR(30),
    email            VARCHAR(150),
    clave            VARCHAR(100) NOT NULL
);

CREATE TABLE inquilino (
    id_inquilino     INT AUTO_INCREMENT PRIMARY KEY,
    dni              VARCHAR(15) NOT NULL UNIQUE,
    nombre           VARCHAR(100) NOT NULL,
    apellido         VARCHAR(100) NOT NULL,
    telefono         VARCHAR(30),
    email            VARCHAR(150)
);

-- =========================================================
-- Datos iniciales de prueba
-- =========================================================

INSERT INTO propietario (dni, nombre, apellido, telefono, email, clave) VALUES
('30111222', 'Marta',   'Gonzalez', '2664111222', 'marta.gonzalez@mail.com', '1234'),
('28555666', 'Ricardo', 'Fernandez', '2664333444', 'ricardo.fernandez@mail.com', '1234'),
('35777888', 'Lucia',   'Torres',   '2664555666', 'lucia.torres@mail.com', '1234');

INSERT INTO inquilino (dni, nombre, apellido, telefono, email) VALUES
('32999000', 'Diego',   'Ramirez',  '2664777888', 'diego.ramirez@mail.com'),
('31222333', 'Carla',   'Suarez',   '2664999000', 'carla.suarez@mail.com'),
('40123456', 'Nahuel',  'Ortiz',    '2664112233', 'nahuel.ortiz@mail.com');