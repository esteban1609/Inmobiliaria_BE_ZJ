-- =========================================================
-- Reservas Temporales - Script de creación e inicialización
-- Motor: MySQL
-- Entrega 1: Propietario e Inquilino
-- =========================================================
 
CREATE DATABASE IF NOT EXISTS reservas_temporales;
USE reservas_temporales;
 
DROP TABLE IF EXISTS propietario;
DROP TABLE IF EXISTS inquilino;
 
-- =========================================================
-- Creación de tablas
-- =========================================================
 
CREATE TABLE propietario (
    id_propietario   INT AUTO_INCREMENT PRIMARY KEY,
    dni              VARCHAR(15) NOT NULL UNIQUE,
    nombre           VARCHAR(100) NOT NULL,
    apellido         VARCHAR(100) NOT NULL,
    telefono         VARCHAR(30),
    email            VARCHAR(150),
    clave            VARCHAR(100) NOT NULL,
    estado           BOOLEAN NOT NULL DEFAULT TRUE
);
 
CREATE TABLE inquilino (
    id_inquilino     INT AUTO_INCREMENT PRIMARY KEY,
    dni              VARCHAR(15) NOT NULL UNIQUE,
    nombre           VARCHAR(100) NOT NULL,
    apellido         VARCHAR(100) NOT NULL,
    telefono         VARCHAR(30),
    email            VARCHAR(150),
    estado           BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE inmueble (
    id_inmueble INT AUTO_INCREMENT PRIMARY KEY,
    direccion VARCHAR(150) NOT NULL,
    cupo INT NOT NULL,
    precio_por_dia DECIMAL(10,2) NOT NULL,
    porcentaje_reserva DECIMAL(5,2) NOT NULL,
    latitud DECIMAL(10,7),
    longitud DECIMAL(10,7),
    id_propietario INT NOT NULL,
    estado BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_inmueble_propietario
        FOREIGN KEY (id_propietario)
        REFERENCES propietario(id_propietario)
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


INSERT INTO inmueble
(direccion, cupo, precio_por_dia, porcentaje_reserva, latitud, longitud, id_propietario, estado)
VALUES
('Av. Illia 850', 4, 45000.00, 20.00, -33.3017, -66.3378, 1, TRUE);

INSERT INTO inmueble
(direccion, cupo, precio_por_dia, porcentaje_reserva, latitud, longitud, id_propietario, estado)
VALUES
('San Martín 1250', 6, 65000.00, 25.00, -33.2950, -66.3350, 2, TRUE);

INSERT INTO inmueble
(direccion, cupo, precio_por_dia, porcentaje_reserva, latitud, longitud, id_propietario, estado)
VALUES
('Pringles 420', 3, 38000.00, 15.00, -33.2980, -66.3400, 3, FALSE);