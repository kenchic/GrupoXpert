# Guía de Frontend para MAUI Blazor Hybrid - GrupoXpert

Esta guía documenta las mejores prácticas y soluciones aplicadas para asegurar un diseño premium y funcional en la aplicación móvil MAUI, evitando problemas comunes de renderizado y usabilidad.

## 1. Renderizado de Iconos (Material Icons)

En dispositivos móviles (especialmente Android), los iconos de fuentes externas pueden fallar al cargar o mostrarse como texto plano. Para evitar esto, se deben seguir estas reglas:

### Configuración en index.html
Siempre incluir el link oficial de Google Fonts:
```html
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
```

### Definición en app.css
Se debe definir un `@font-face` con formato **TTF** como respaldo, ya que es el formato más compatible con los WebViews de Android.

```css
@font-face {
  font-family: 'Material Icons';
  font-style: normal;
  font-weight: 400;
  src: url(https://fonts.gstatic.com/s/materialicons/v145/flUhRq6tzZclQEJ-Vdg-IuiaDsNZ.ttf) format('truetype');
  font-display: block;
}

.material-icons {
  font-family: 'Material Icons' !important;
  font-weight: normal;
  font-style: normal;
  font-size: 24px;
  line-height: 1;
  letter-spacing: normal;
  text-transform: none;
  display: inline-block;
  white-space: nowrap;
  word-wrap: normal;
  direction: ltr;
  /* Soporte de ligaduras (Importante para que nombres como 'home' funcionen) */
  font-feature-settings: 'liga';
  -webkit-font-feature-settings: 'liga';
  -webkit-font-smoothing: antialiased;
}
```

## 2. Manejo de Áreas Seguras (Safe Areas)

Para evitar que los elementos de la UI colisionen con el Notch (cámara) o los indicadores del sistema en la parte inferior:

### Barra Superior (Navbar)
Usar `env(safe-area-inset-top)` con un fallback para dispositivos que no lo reporten correctamente:
```css
.gx-mobile-topbar {
    padding-top: env(safe-area-inset-top, 24px);
    height: calc(var(--navbar-height) + env(safe-area-inset-top, 24px));
}
```

### Barra Inferior (Tab Bar)
Incrementar el padding inferior para separar los labels del borde de la pantalla:
```css
.gx-tab-bar {
    padding-bottom: env(safe-area-inset-bottom, 32px);
    height: calc(var(--tab-bar-height) + env(safe-area-inset-bottom, 32px));
}
```

## 3. Estética Premium (Glassmorphism)
Para lograr el aspecto premium estilo LinkedIn, se utiliza transparencia y desenfoque:
```css
background: rgba(255, 255, 255, 0.95);
backdrop-filter: blur(10px);
-webkit-backdrop-filter: blur(10px);
```
