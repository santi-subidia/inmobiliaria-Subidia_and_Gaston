-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 13-09-2025 a las 13:24:47
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `inmueble_id` bigint(20) UNSIGNED NOT NULL,
  `inquilino_id` bigint(20) UNSIGNED NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin_original` date NOT NULL,
  `fecha_fin_efectiva` date DEFAULT NULL,
  `monto_mensual` decimal(12,2) NOT NULL,
  `estado` enum('VIGENTE','FINALIZADO','RESCINDIDO') NOT NULL DEFAULT 'VIGENTE',
  `renovado_de_id` bigint(20) UNSIGNED DEFAULT NULL,
  `creado_por` bigint(20) UNSIGNED DEFAULT NULL,
  `creado_at` datetime NOT NULL DEFAULT current_timestamp(),
  `finalizado_por` bigint(20) UNSIGNED DEFAULT NULL,
  `finalizado_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`id`, `inmueble_id`, `inquilino_id`, `fecha_inicio`, `fecha_fin_original`, `fecha_fin_efectiva`, `monto_mensual`, `estado`, `renovado_de_id`, `creado_por`, `creado_at`, `finalizado_por`, `finalizado_at`) VALUES
(2, 2, 4, '2025-09-12', '2025-11-12', NULL, 80000.00, 'RESCINDIDO', NULL, NULL, '2025-09-12 18:25:51', NULL, '2025-09-12 18:25:51'),
(3, 4, 5, '2025-09-12', '2026-01-16', NULL, 50000.00, 'FINALIZADO', NULL, NULL, '2025-09-12 19:39:37', NULL, '2025-09-12 19:39:37');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `propietario_id` bigint(20) UNSIGNED NOT NULL,
  `tipo_id` bigint(20) UNSIGNED NOT NULL,
  `uso` enum('RESIDENCIAL','COMERCIAL') NOT NULL,
  `ambientes` int(11) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `coordenada_lat` decimal(9,6) DEFAULT NULL,
  `coordenada_lon` decimal(9,6) DEFAULT NULL,
  `precio_sugerido` decimal(12,2) NOT NULL,
  `suspendido` tinyint(1) NOT NULL DEFAULT 0,
  `observaciones` text DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `fecha_eliminacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `propietario_id`, `tipo_id`, `uso`, `ambientes`, `direccion`, `coordenada_lat`, `coordenada_lon`, `precio_sugerido`, `suspendido`, `observaciones`, `created_at`, `updated_at`, `fecha_eliminacion`) VALUES
(1, 2, 2, 'COMERCIAL', 3, 'Betbeder 44', 80.000000, 110.000000, 70000000.00, 0, 'Casa de Familia', '2025-09-08 21:38:45', '2025-09-12 00:07:00', NULL),
(2, 25, 1, 'COMERCIAL', 4, 'sanlui 200', 70.000000, 90.000000, 60000000.00, 0, 'ambiente familiar', '2025-09-08 21:49:08', '2025-09-08 22:36:42', NULL),
(3, 20, 1, 'RESIDENCIAL', 3, 'Calle falsa 13456', 70.000000, -30.000000, 80000000.00, 1, 'Nada que agregar', '2025-09-12 00:10:05', '2025-09-12 00:11:02', NULL),
(4, 10, 2, 'RESIDENCIAL', 5, 'calle falsa 45678915', 50.000000, 80.000000, 900000.00, 0, NULL, '2025-09-12 19:20:54', '2025-09-12 19:20:54', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `dni` varchar(20) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `direccion` varchar(255) DEFAULT NULL,
  `fecha_eliminacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `dni`, `apellido`, `nombre`, `telefono`, `email`, `created_at`, `updated_at`, `direccion`, `fecha_eliminacion`) VALUES
(2, '37090426', 'sosa', 'fgaston', '8541654', 'adfgasdf@gmail.com', '2025-09-01 23:07:41', '2025-09-01 23:07:41', 'asdasd51651', NULL),
(3, '370904260', 'german', 'paomf', '6514651', 'agefasd@gmail.com', '2025-09-01 23:08:24', '2025-09-01 23:08:24', '51asfasf651', NULL),
(4, '30111222', 'Perez', 'Juan', '2645123456', 'juanperez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Av. Libertador 123', NULL),
(5, '32544321', 'Gomez', 'Maria', '2645234567', 'mariagomez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Calle San Martín 456', NULL),
(6, '28456789', 'Lopez', 'Carlos', '2645345678', 'carloslopez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Mitre 789', NULL),
(7, '31234567', 'Fernandez', 'Ana', '2645456789', 'anafernandez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Belgrano 321', NULL),
(8, '29876543', 'Diaz', 'Luis', '2645567890', 'luisdiaz@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', '25 de Mayo 654', NULL),
(9, '27345678', 'Martinez', 'Lucia', '2645678901', 'luciamartinez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Av. Rivadavia 987', NULL),
(10, '34123456', 'Sanchez', 'Pedro', '2645789012', 'pedrosanchez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'San Juan 111', NULL),
(11, '28901234', 'Romero', 'Sofia', '2645890123', 'sofiaromero@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Rawson 222', NULL),
(12, '30567891', 'Ruiz', 'Diego', '2645901234', 'diegoruiz@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Catamarca 333', NULL),
(13, '32123456', 'Torres', 'Valentina', '2646012345', 'valentinatorres@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'La Rioja 444', NULL),
(14, '27654321', 'Acosta', 'Mateo', '2646123456', 'mateoacosta@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'San Luis 555', NULL),
(15, '33456789', 'Gutierrez', 'Camila', '2646234567', 'camilagutierrez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Cordoba 666', NULL),
(16, '29223344', 'Silva', 'Joaquin', '2646345678', 'joaquinsilva@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Mendoza 777', NULL),
(17, '31445566', 'Molina', 'Martina', '2646456789', 'martinamolina@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Buenos Aires 888', NULL),
(18, '30778899', 'Castro', 'Nicolas', '2646567890', 'nicolascastro@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Corrientes 999', NULL),
(19, '32889900', 'Rojas', 'Florencia', '2646678901', 'florenciarojas@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Entre Rios 101', NULL),
(20, '29665544', 'Dominguez', 'Tomas', '2646789012', 'tomasdominguez@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Santa Fe 202', NULL),
(21, '33998877', 'Herrera', 'Agustina', '2646890123', 'agustinaherrera@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Jujuy 303', NULL),
(22, '30881234', 'Morales', 'Franco', '2646901234', 'francomorales@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Salta 404', NULL),
(23, '31772211', 'Navarro', 'Julieta', '2647012345', 'julietanavarro@mail.com', '2025-09-01 20:10:15', '2025-09-01 20:10:15', 'Tucuman 505', NULL),
(24, '65198444', 'Sosita', 'Gaston', '2657656565', 'gaston@gmail.com', '2025-09-04 19:08:36', '2025-09-04 19:15:28', 'calle falsa 123', NULL),
(25, '401564783', 'Rojelio', 'Mori', '2665789457', 'Rojelio@gmail.com', '2025-09-12 00:09:08', '2025-09-12 00:09:08', 'calle falsa 7891524', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `contrato_id` bigint(20) UNSIGNED NOT NULL,
  `numero_pago` int(10) UNSIGNED NOT NULL,
  `fecha_pago` date NOT NULL,
  `concepto` varchar(255) NOT NULL,
  `importe` decimal(12,2) NOT NULL,
  `estado` enum('ACTIVO','ANULADO') NOT NULL DEFAULT 'ACTIVO',
  `creado_por` bigint(20) UNSIGNED NOT NULL,
  `creado_at` datetime NOT NULL DEFAULT current_timestamp(),
  `anulado_por` bigint(20) UNSIGNED DEFAULT NULL,
  `anulado_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `dni` varchar(20) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `direccion_contacto` varchar(255) DEFAULT NULL,
  `fecha_eliminacion` date DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`id`, `dni`, `apellido`, `nombre`, `telefono`, `email`, `direccion_contacto`, `fecha_eliminacion`, `created_at`, `updated_at`) VALUES
(1, '65198444', 'Sosita', 'Gaston', '2657656565', 'gaston@gmail.com', 'calle falsa 123', NULL, '2025-08-28 20:14:14', '2025-09-04 19:15:28'),
(2, '37090426', 'Gaston', 'sosa', '65165', 'adgadf@gmail.com', 'asdasfd651651', NULL, '2025-09-01 23:10:52', '2025-09-01 23:10:52'),
(4, '30111222', 'Perez', 'Juan', '2645123456', 'juanperez@mail.com', 'Av. Libertador 123', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(5, '32544321', 'Gomez', 'Maria', '2645234567', 'mariagomez@mail.com', 'Calle San Martín 456', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(6, '28456789', 'Lopez', 'Carlos', '2645345678', 'carloslopez@mail.com', 'Mitre 789', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(7, '31234567', 'Fernandez', 'Ana', '2645456789', 'anafernandez@mail.com', 'Belgrano 321', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(8, '29876543', 'Diaz', 'Luis', '2645567890', 'luisdiaz@mail.com', '25 de Mayo 654', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(9, '27345678', 'Martinez', 'Lucia', '2645678901', 'luciamartinez@mail.com', 'Av. Rivadavia 987', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(10, '34123456', 'Sanchez', 'Pedro', '2645789012', 'pedrosanchez@mail.com', 'San Juan 111', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(11, '28901234', 'Romero', 'Sofia', '2645890123', 'sofiaromero@mail.com', 'Rawson 222', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(12, '30567891', 'Ruiz', 'Diego', '2645901234', 'diegoruiz@mail.com', 'Catamarca 333', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(13, '32123456', 'Torres', 'Valentina', '2646012345', 'valentinatorres@mail.com', 'La Rioja 444', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(14, '27654321', 'Acosta', 'Mateo', '2646123456', 'mateoacosta@mail.com', 'San Luis 555', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(15, '33456789', 'Gutierrez', 'Camila', '2646234567', 'camilagutierrez@mail.com', 'Cordoba 666', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(16, '29223344', 'Silva', 'Joaquin', '2646345678', 'joaquinsilva@mail.com', 'Mendoza 777', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(17, '31445566', 'Molina', 'Martina', '2646456789', 'martinamolina@mail.com', 'Buenos Aires 888', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(18, '30778899', 'Castro', 'Nicolas', '2646567890', 'nicolascastro@mail.com', 'Corrientes 999', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(19, '32889900', 'Rojas', 'Florencia', '2646678901', 'florenciarojas@mail.com', 'Entre Rios 101', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(20, '29665544', 'Dominguez', 'Tomas', '2646789012', 'tomasdominguez@mail.com', 'Santa Fe 202', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(21, '33998877', 'Herrera', 'Agustina', '2646890123', 'agustinaherrera@mail.com', 'Jujuy 303', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(22, '30881234', 'Morales', 'Franco', '2646901234', 'francomorales@mail.com', 'Salta 404', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(23, '31772211', 'Navarro', 'Julieta', '2647012345', 'julietanavarro@mail.com', 'Tucuman 505', NULL, '2025-09-01 20:12:42', '2025-09-01 20:12:42'),
(24, '651641', 'sososa', 'asfasf', '6515165165', 'asfa@gmail.com', '1as6f51asf4', NULL, '2025-09-04 19:16:57', '2025-09-04 19:16:57'),
(25, '43621175', 'Carreño', 'Florencia', '51651651', 'flor7108@gmail.com', 'sanjuan 1000', NULL, '2025-09-08 21:48:09', '2025-09-08 21:48:09');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipos_inmueble`
--

CREATE TABLE `tipos_inmueble` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `fecha_eliminacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipos_inmueble`
--

INSERT INTO `tipos_inmueble` (`id`, `nombre`, `descripcion`, `activo`, `created_at`, `updated_at`, `fecha_eliminacion`) VALUES
(1, 'Casa', 'Vivienda unifamiliar', 1, '2025-09-08 18:32:37', '2025-09-08 18:32:37', NULL),
(2, 'Departamento', 'Unidad en edificio', 1, '2025-09-08 18:32:37', '2025-09-08 18:32:37', NULL),
(3, 'Local', 'Comercial a la calle', 1, '2025-09-08 18:32:37', '2025-09-08 18:32:37', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `email` varchar(255) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `avatar_url` varchar(500) DEFAULT NULL,
  `rol` enum('ADMIN','EMPLEADO') NOT NULL DEFAULT 'EMPLEADO',
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_contratos_renovado_de` (`renovado_de_id`),
  ADD KEY `fk_contratos_finalizado_por` (`finalizado_por`),
  ADD KEY `ix_contratos_inmueble` (`inmueble_id`),
  ADD KEY `ix_contratos_inquilino` (`inquilino_id`),
  ADD KEY `ix_contratos_estado` (`estado`),
  ADD KEY `ix_contratos_fechas` (`fecha_inicio`,`fecha_fin_original`),
  ADD KEY `ix_contratos_creado_por` (`creado_por`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ix_inmuebles_propietario` (`propietario_id`),
  ADD KEY `ix_inmuebles_tipo` (`tipo_id`),
  ADD KEY `ix_inmuebles_suspendido` (`suspendido`),
  ADD KEY `idx_inm_filtros` (`uso`,`tipo_id`,`ambientes`,`precio_sugerido`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_inquilinos_dni` (`dni`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_pagos_numero_por_contrato` (`contrato_id`,`numero_pago`),
  ADD KEY `fk_pagos_creado_por` (`creado_por`),
  ADD KEY `fk_pagos_anulado_por` (`anulado_por`),
  ADD KEY `ix_pagos_contrato` (`contrato_id`),
  ADD KEY `ix_pagos_estado` (`estado`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_propietarios_dni` (`dni`);

--
-- Indices de la tabla `tipos_inmueble`
--
ALTER TABLE `tipos_inmueble`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_tipos_inmueble_nombre` (`nombre`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_usuarios_email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT de la tabla `tipos_inmueble`
--
ALTER TABLE `tipos_inmueble`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `fk_contratos_creado_por` FOREIGN KEY (`creado_por`) REFERENCES `usuarios` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_finalizado_por` FOREIGN KEY (`finalizado_por`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_inmueble` FOREIGN KEY (`inmueble_id`) REFERENCES `inmuebles` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_inquilino` FOREIGN KEY (`inquilino_id`) REFERENCES `inquilinos` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_renovado_de` FOREIGN KEY (`renovado_de_id`) REFERENCES `contratos` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `fk_inm_propietario` FOREIGN KEY (`propietario_id`) REFERENCES `propietarios` (`id`),
  ADD CONSTRAINT `fk_inm_tipo` FOREIGN KEY (`tipo_id`) REFERENCES `tipos_inmueble` (`id`),
  ADD CONSTRAINT `fk_inmuebles_propietario` FOREIGN KEY (`propietario_id`) REFERENCES `propietarios` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_inmuebles_tipo` FOREIGN KEY (`tipo_id`) REFERENCES `tipos_inmueble` (`id`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `fk_pagos_anulado_por` FOREIGN KEY (`anulado_por`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pagos_contrato` FOREIGN KEY (`contrato_id`) REFERENCES `contratos` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pagos_creado_por` FOREIGN KEY (`creado_por`) REFERENCES `usuarios` (`id`) ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
