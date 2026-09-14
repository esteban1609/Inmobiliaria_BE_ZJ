-- =========================================================
-- Reservas Temporales - Script de creación e inicialización
-- Motor: MySQL
-- Entrega Completa: Propietario, Inquilino, Inmuebles y Reserva
-- =========================================================

CREATE DATABASE IF NOT EXISTS reservas_temporales;
USE reservas_temporales;

-- Eliminar tablas en orden inverso a las dependencias de FK
DROP TABLE IF EXISTS reserva;
DROP TABLE IF EXISTS inmuebles;
DROP TABLE IF EXISTS inquilino;
DROP TABLE IF EXISTS propietario;

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
    id_inmueble         INT AUTO_INCREMENT PRIMARY KEY,
    direccion           VARCHAR(150) NOT NULL,
    cupo                INT NOT NULL,
    precio_por_dia      DECIMAL(10,2) NOT NULL,
    porcentaje_reserva  DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    latitud             DECIMAL(10,7),
    longitud            DECIMAL(10,7),
    id_propietario      INT NOT NULL,
    id_tipo             INT NOT NULL,
    estado              BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_inmueble_propietario
        FOREIGN KEY (id_propietario)
        REFERENCES propietario(id_propietario)
        ON DELETE RESTRICT
        ON UPDATE CASCADE

    CONSTRAINT fk_inmueble_tipo
        FOREIGN KEY (id_tipo)
        REFERENCES tipoinmueble(id_tipo)
        ON DELETE RESTRICT
        ON UPDATE CASCADE    
);

CREATE TABLE reserva (
    id_reserva          INT AUTO_INCREMENT PRIMARY KEY,
    id_inquilino        INT NOT NULL,
    id_inmueble         INT NOT NULL,
    monto_dia           DECIMAL(10,2) NOT NULL,
    fecha_desde         DATETIME NOT NULL,
    fecha_hasta         DATETIME NOT NULL,
    estado              BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_reserva_inquilino
        FOREIGN KEY (id_inquilino)
        REFERENCES inquilino(id_inquilino)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_reserva_inmueble
        FOREIGN KEY (id_inmueble)
        REFERENCES inmueble(id_inmueble)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
);

CREATE TABLE IF NOT EXISTS imagenes (
    id_imagen INT NOT NULL AUTO_INCREMENT,
    inmueble_id INT NOT NULL,
    url VARCHAR(255) NOT NULL,
    PRIMARY KEY (id_imagen),

    CONSTRAINT fk_imagen_inmueble
        FOREIGN KEY (inmueble_id)
        REFERENCES inmueble(id_inmueble)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);



CREATE TABLE TipoInmueble (
    id_tipo INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Estado BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE pago (
    id_pago          INT AUTO_INCREMENT PRIMARY KEY,
    id_reserva       INT NOT NULL,
    concepto         VARCHAR(200) NOT NULL,
    fecha_pago       DATE NOT NULL,
    importe          DECIMAL(10,2) NOT NULL,
    estado           BOOLEAN NOT NULL DEFAULT TRUE,  
    CONSTRAINT fk_pago_reserva FOREIGN KEY (id_reserva) REFERENCES reserva(id_reserva)
);

CREATE TABLE usuario (
    id_usuario       INT AUTO_INCREMENT PRIMARY KEY,
    nombre           VARCHAR(100) NOT NULL,
    apellido         VARCHAR(100) NOT NULL,
    email            VARCHAR(150) NOT NULL UNIQUE,
    clave            VARCHAR(255) NOT NULL,  
    rol              VARCHAR(20) NOT NULL,   
    avatar           VARCHAR(255),
    estado           BOOLEAN NOT NULL DEFAULT TRUE
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
(direccion, cupo, precio_por_dia, porcentaje_reserva, latitud, longitud, id_propietario, id_tipo, estado)
VALUES
('Pringles 420', 3, 38000.00, 15.00, -33.2980000, -66.3400000, 3, 2, TRUE),
('San Martín 1250', 6, 65000.00, 25.00, -33.2950000, -66.3350000, 2, 1, TRUE),
('Av. Illia 850', 4, 45000.00, 20.00, -33.3017000, -66.3378000, 1, 3, TRUE);

INSERT INTO reserva 
(id_inquilino, id_inmueble, monto_dia, fecha_desde, fecha_hasta, estado) 
VALUES
(1, 1, 45000.00, '2026-10-01 12:00:00', '2026-10-07 10:00:00', TRUE),
(2, 2, 65000.00, '2026-11-15 12:00:00', '2026-11-20 10:00:00', TRUE);


INSERT INTO TipoInmueble (Nombre, Estado)
VALUES
('Casa', TRUE),
('Departamento', TRUE),
('Local Comercial', TRUE),
('Terreno', TRUE);