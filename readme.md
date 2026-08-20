# Reservas Temporales

> Sistema para la gestión de alquileres temporarios de propiedades inmuebles que realiza una agencia inmobiliaria: propietarios, inmuebles, inquilinos, reservas y pagos.

---

## 👥 Integrantes del Grupo

* **Barroso Esteban** - *Estebanbarroso037@gmail.com* - [@esteban1609](https://github.com/esteban1609) - Discord: `venux7076`
* **Josemir Zaleh** - *josemirzaleh45@gmail.com* - [@josemir01](https://github.com/josemir01) - Discord: `pendiente`

---

## 📐 Modelado de Datos

A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación:

### Diagrama Entidad-Relación (DER) / Diagrama de Clases

![Diagrama del Proyecto](./docs/der.png)

> **Nota:** reemplazar `./docs/der.png` por la ruta real del diagrama dentro del repositorio (por ejemplo, agregándolo a una carpeta `/docs` o `/img`).

<details>
<summary>Ver diagrama en código Mermaid (Opcional)</summary>

```mermaid
erDiagram
    PROPIETARIO ||--o{ INMUEBLE : posee
    INMUEBLE ||--o{ RESERVA : "es reservado en"
    INQUILINO ||--o{ RESERVA : realiza
    RESERVA ||--o{ PAGO : tiene
    USUARIO ||--o{ RESERVA : registra
```

</details>
