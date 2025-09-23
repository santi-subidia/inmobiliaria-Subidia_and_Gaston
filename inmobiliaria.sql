-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Sep 23, 2025 at 10:57 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Table structure for table `contratos`
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
  `creado_por` bigint(20) UNSIGNED NOT NULL,
  `creado_at` datetime NOT NULL DEFAULT current_timestamp(),
  `finalizado_por` bigint(20) UNSIGNED DEFAULT NULL,
  `finalizado_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `contratos`
--

INSERT INTO `contratos` (`id`, `inmueble_id`, `inquilino_id`, `fecha_inicio`, `fecha_fin_original`, `fecha_fin_efectiva`, `monto_mensual`, `estado`, `creado_por`, `creado_at`, `finalizado_por`, `finalizado_at`) VALUES
(12, 14, 25, '2025-10-01', '2027-09-01', NULL, 120000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(13, 15, 26, '2025-11-01', '2027-10-01', NULL, 135000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(14, 16, 27, '2025-12-01', '2027-11-01', NULL, 110000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(15, 17, 28, '2026-01-01', '2027-12-01', NULL, 140000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(16, 18, 29, '2026-02-01', '2028-01-01', NULL, 125000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(17, 19, 30, '2026-03-01', '2028-02-01', NULL, 150000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(18, 20, 31, '2026-04-01', '2028-03-01', NULL, 115000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(19, 21, 32, '2026-05-01', '2028-04-01', NULL, 160000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL),
(20, 22, 33, '2026-06-01', '2028-05-01', '2025-09-23', 130000.00, 'RESCINDIDO', 1, '2025-09-22 15:35:46', 1, '2025-09-23 13:07:13'),
(21, 23, 34, '2026-07-01', '2028-06-01', NULL, 145000.00, 'VIGENTE', 1, '2025-09-22 15:35:46', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `imagenes`
--

CREATE TABLE `imagenes` (
  `id` int(11) NOT NULL,
  `inmueble_id` bigint(20) UNSIGNED NOT NULL,
  `url` varchar(500) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `inmuebles`
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
  `portada_url` varchar(500) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `fecha_eliminacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `propietario_id`, `tipo_id`, `uso`, `ambientes`, `direccion`, `coordenada_lat`, `coordenada_lon`, `precio_sugerido`, `suspendido`, `observaciones`, `portada_url`, `created_at`, `updated_at`, `fecha_eliminacion`) VALUES
(14, 26, 1, 'RESIDENCIAL', 3, 'Av. Siempre Viva 742', -34.603722, -58.381592, 150000.00, 0, 'Casa familiar con jardín amplio.', NULL, '2025-09-22 11:16:26', '2025-09-23 15:08:40', NULL),
(15, 27, 2, 'RESIDENCIAL', 2, 'Calle Falsa 123, Piso 4 Dto B', -34.615803, -58.433298, 85000.00, 0, 'Departamento luminoso en excelente ubicación.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(16, 28, 3, 'COMERCIAL', 1, 'Av. Corrientes 3500, Local 5', -34.603932, -58.410146, 220000.00, 0, 'Local comercial a la calle con gran vidriera.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(17, 29, 1, 'RESIDENCIAL', 4, 'San Martín 450', -34.598900, -58.377300, 180000.00, 0, 'Casa de dos plantas con patio trasero.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(18, 30, 2, 'RESIDENCIAL', 1, 'Belgrano 1200, Piso 3', -34.620500, -58.385700, 70000.00, 0, 'Monoambiente ideal para estudiante.', NULL, '2025-09-22 11:16:26', '2025-09-23 15:08:44', NULL),
(19, 31, 3, 'COMERCIAL', 2, 'Florida 500, Local 2', -34.603400, -58.377800, 260000.00, 0, 'Local en zona de alto tránsito peatonal.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(20, 32, 1, 'RESIDENCIAL', 5, 'Mitre 2300', -34.596700, -58.399900, 250000.00, 0, 'Casa quinta con parque y pileta.', NULL, '2025-09-22 11:16:26', '2025-09-23 15:08:47', NULL),
(21, 33, 2, 'RESIDENCIAL', 3, 'Rivadavia 3300, Piso 2', -34.608500, -58.407400, 120000.00, 0, 'Departamento de 3 ambientes con balcón.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(22, 34, 3, 'COMERCIAL', 1, 'Santa Fe 1500, Local A', -34.596500, -58.392100, 300000.00, 0, 'Local ideal para cafetería o boutique.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(23, 35, 1, 'RESIDENCIAL', 4, 'Dorrego 2100', -34.589200, -58.435600, 190000.00, 0, 'Chalet con garaje doble y jardín frontal.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(24, 35, 1, 'RESIDENCIAL', 4, 'Calle Falsa 785', -50.467800, 80.467800, 80000.00, 0, NULL, NULL, '2025-09-23 15:09:42', '2025-09-23 15:09:42', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `dni` varchar(20) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `fecha_eliminacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `dni`, `apellido`, `nombre`, `telefono`, `email`, `fecha_eliminacion`) VALUES
(25, '32145678', 'Gómez', 'Lucía', '+54 9 11 2233-4455', 'lucia.gomez@example.com', NULL),
(26, '29567345', 'Fernández', 'Matías', '+54 9 11 3344-5566', 'matias.fernandez@example.com', NULL),
(27, '30891234', 'Rojas', 'Carolina', '+54 9 11 1122-3344', 'carolina.rojas@example.com', NULL),
(28, '27456321', 'Pereyra', 'Julián', '+54 9 11 5566-7788', 'julian.pereyra@example.com', NULL),
(29, '33214567', 'Sosa', 'Micaela', '+54 9 11 6677-8899', 'micaela.sosa@example.com', NULL),
(30, '30123987', 'López', 'Agustín', '+54 9 11 7788-9900', 'agustin.lopez@example.com', NULL),
(31, '28903456', 'Navarro', 'Paula', '+54 9 11 8899-0011', 'paula.navarro@example.com', NULL),
(32, '31567890', 'Silva', 'Tomás', '+54 9 11 9900-1122', 'tomas.silva@example.com', NULL),
(33, '29988776', 'Méndez', 'Florencia', '+54 9 11 1234-5678', 'florencia.mendez@example.com', NULL),
(34, '28765432', 'Cabrera', 'Rodrigo', '+54 9 11 2345-6789', 'rodrigo.cabrera@example.com', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `pagos`
--

CREATE TABLE `pagos` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `contrato_id` bigint(20) UNSIGNED NOT NULL,
  `numero_pago` int(10) UNSIGNED NOT NULL,
  `fecha_pago` date NOT NULL,
  `concepto` varchar(255) NOT NULL,
  `importe` decimal(12,2) NOT NULL,
  `estado` enum('Pagado','Anulado','Pendiente') NOT NULL DEFAULT 'Pagado',
  `creado_por` bigint(20) UNSIGNED NOT NULL,
  `creado_at` datetime NOT NULL DEFAULT current_timestamp(),
  `anulado_por` bigint(20) UNSIGNED DEFAULT NULL,
  `anulado_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `pagos`
--

INSERT INTO `pagos` (`id`, `contrato_id`, `numero_pago`, `fecha_pago`, `concepto`, `importe`, `estado`, `creado_por`, `creado_at`, `anulado_por`, `anulado_at`) VALUES
(14, 12, 1, '2025-09-22', 'Adelanto Octubre', 120000.00, 'Pagado', 1, '2025-09-22 19:36:42', NULL, NULL),
(15, 12, 2, '2025-09-22', 'completo', 120000.00, 'Pagado', 1, '2025-09-22 19:37:04', NULL, NULL),
(16, 20, 1, '2025-09-23', 'Alquiler mes Septiembre', 130000.00, 'Pagado', 1, '2025-09-23 13:43:03', NULL, NULL),
(17, 20, 2, '2025-09-23', 'Adelanto Octubre', 130000.00, 'Anulado', 1, '2025-09-23 13:45:12', 1, '2025-09-23 14:22:39'),
(18, 20, 3, '2025-09-23', 'completo', 2990000.00, 'Pagado', 1, '2025-09-23 13:45:38', NULL, NULL),
(19, 20, 4, '2025-09-23', 'completo', 130000.00, 'Pagado', 1, '2025-09-23 14:23:08', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `propietarios`
--

CREATE TABLE `propietarios` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `dni` varchar(20) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `fecha_eliminacion` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `propietarios`
--

INSERT INTO `propietarios` (`id`, `dni`, `apellido`, `nombre`, `telefono`, `email`, `fecha_eliminacion`) VALUES
(26, '20123456', 'Pérez', 'Jorge', '+54 9 11 3344-5566', 'jorge.perez@example.com', NULL),
(27, '22567890', 'Ramírez', 'Silvana', '+54 9 11 2233-4455', 'silvana.ramirez@example.com', NULL),
(28, '21345678', 'García', 'Héctor', '+54 9 11 4455-6677', 'hector.garcia@example.com', NULL),
(29, '23987654', 'Domínguez', 'Laura', '+54 9 11 5566-7788', 'laura.dominguez@example.com', NULL),
(30, '24678901', 'Martínez', 'Ricardo', '+54 9 11 6677-8899', 'ricardo.martinez@example.com', NULL),
(31, '25432109', 'Suárez', 'Verónica', '+54 9 11 7788-9900', 'veronica.suarez@example.com', NULL),
(32, '26890123', 'Herrera', 'Fernando', '+54 9 11 8899-0011', 'fernando.herrera@example.com', NULL),
(33, '27891234', 'Torres', 'Gabriela', '+54 9 11 9900-1122', 'gabriela.torres@example.com', NULL),
(34, '28345678', 'Castro', 'Sergio', '+54 9 11 1234-5678', 'sergio.castro@example.com', NULL),
(35, '29765432', 'Morales', 'Patricia', '+54 9 11 2345-6789', 'patricia.morales@example.com', NULL),
(36, '46801648', 'Solari', 'Sandro', '2664789413', 'Jorge@gmail.com', '2025-09-23'),
(37, '47156234', 'Solari', 'Gaston', '2664784618', 'SolariGaston@gmail.com', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `rol_usuarios`
--

CREATE TABLE `rol_usuarios` (
  `id` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `descripcion` varchar(150) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `rol_usuarios`
--

INSERT INTO `rol_usuarios` (`id`, `nombre`, `descripcion`, `created_at`, `updated_at`) VALUES
(1, 'Admin', 'Acceso completo al sistema', '2025-09-18 16:01:50', '2025-09-18 16:01:50'),
(2, 'Empleado', 'Empleado interno con acceso a funciones operativas', '2025-09-18 16:01:50', '2025-09-18 16:01:50'),
(3, 'Usuario', 'Usuario general del sistema', '2025-09-18 16:01:50', '2025-09-18 16:01:50'),
(4, 'Propietario', 'Propietario de un inmueble', '2025-09-18 16:01:50', '2025-09-18 16:01:50'),
(5, 'Inquilino', 'Inquilino con contrato activo', '2025-09-18 16:01:50', '2025-09-18 16:01:50');

-- --------------------------------------------------------

--
-- Table structure for table `tipos_inmueble`
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
-- Dumping data for table `tipos_inmueble`
--

INSERT INTO `tipos_inmueble` (`id`, `nombre`, `descripcion`, `activo`, `created_at`, `updated_at`, `fecha_eliminacion`) VALUES
(1, 'Casa', 'Vivienda unifamiliar', 1, '2025-09-08 18:32:37', '2025-09-08 18:32:37', NULL),
(2, 'Departamento', 'Unidad en edificio', 1, '2025-09-08 18:32:37', '2025-09-08 18:32:37', NULL),
(3, 'Local', 'Comercial a la calle', 1, '2025-09-08 18:32:37', '2025-09-08 18:32:37', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `usuarios`
--

CREATE TABLE `usuarios` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `email` varchar(255) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `avatar_url` varchar(500) DEFAULT NULL,
  `rol_id` int(11) NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `usuarios`
--

INSERT INTO `usuarios` (`id`, `email`, `password_hash`, `nombre`, `apellido`, `telefono`, `avatar_url`, `rol_id`, `is_active`, `created_at`, `updated_at`) VALUES
(1, 'admin1@example.com', 'hash123', 'Juan', 'Pérez', '+54 9 11 2345-6789', 'https://i.pravatar.cc/150?img=1', 1, 1, '2025-09-18 16:07:01', '2025-09-18 16:07:01');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_contratos_finalizado_por` (`finalizado_por`),
  ADD KEY `ix_contratos_inmueble` (`inmueble_id`),
  ADD KEY `ix_contratos_inquilino` (`inquilino_id`),
  ADD KEY `ix_contratos_estado` (`estado`),
  ADD KEY `ix_contratos_fechas` (`fecha_inicio`,`fecha_fin_original`),
  ADD KEY `ix_contratos_creado_por` (`creado_por`);

--
-- Indexes for table `imagenes`
--
ALTER TABLE `imagenes`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idx_imagenes_inmueble` (`inmueble_id`);

--
-- Indexes for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ix_inmuebles_propietario` (`propietario_id`),
  ADD KEY `ix_inmuebles_tipo` (`tipo_id`),
  ADD KEY `ix_inmuebles_suspendido` (`suspendido`),
  ADD KEY `idx_inm_filtros` (`uso`,`tipo_id`,`ambientes`,`precio_sugerido`);

--
-- Indexes for table `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_inquilinos_dni` (`dni`);

--
-- Indexes for table `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_pagos_numero_por_contrato` (`contrato_id`,`numero_pago`),
  ADD KEY `fk_pagos_creado_por` (`creado_por`),
  ADD KEY `fk_pagos_anulado_por` (`anulado_por`),
  ADD KEY `ix_pagos_contrato` (`contrato_id`),
  ADD KEY `ix_pagos_estado` (`estado`);

--
-- Indexes for table `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_propietarios_dni` (`dni`);

--
-- Indexes for table `rol_usuarios`
--
ALTER TABLE `rol_usuarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nombre` (`nombre`);

--
-- Indexes for table `tipos_inmueble`
--
ALTER TABLE `tipos_inmueble`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_tipos_inmueble_nombre` (`nombre`);

--
-- Indexes for table `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `uq_usuarios_email` (`email`),
  ADD KEY `fk_usuario_rol` (`rol_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT for table `imagenes`
--
ALTER TABLE `imagenes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;

--
-- AUTO_INCREMENT for table `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT for table `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

--
-- AUTO_INCREMENT for table `rol_usuarios`
--
ALTER TABLE `rol_usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `tipos_inmueble`
--
ALTER TABLE `tipos_inmueble`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `fk_contratos_creado_por` FOREIGN KEY (`creado_por`) REFERENCES `usuarios` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_finalizado_por` FOREIGN KEY (`finalizado_por`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_inmueble` FOREIGN KEY (`inmueble_id`) REFERENCES `inmuebles` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_inquilino` FOREIGN KEY (`inquilino_id`) REFERENCES `inquilinos` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_renovado_de` FOREIGN KEY (`renovado_de_id`) REFERENCES `contratos` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Constraints for table `imagenes`
--
ALTER TABLE `imagenes`
  ADD CONSTRAINT `imagenes_ibfk_1` FOREIGN KEY (`inmueble_id`) REFERENCES `inmuebles` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `fk_inm_propietario` FOREIGN KEY (`propietario_id`) REFERENCES `propietarios` (`id`),
  ADD CONSTRAINT `fk_inm_tipo` FOREIGN KEY (`tipo_id`) REFERENCES `tipos_inmueble` (`id`),
  ADD CONSTRAINT `fk_inmuebles_propietario` FOREIGN KEY (`propietario_id`) REFERENCES `propietarios` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_inmuebles_tipo` FOREIGN KEY (`tipo_id`) REFERENCES `tipos_inmueble` (`id`) ON UPDATE CASCADE;

--
-- Constraints for table `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `fk_pagos_anulado_por` FOREIGN KEY (`anulado_por`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pagos_contrato` FOREIGN KEY (`contrato_id`) REFERENCES `contratos` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pagos_creado_por` FOREIGN KEY (`creado_por`) REFERENCES `usuarios` (`id`) ON UPDATE CASCADE;

--
-- Constraints for table `usuarios`
--
ALTER TABLE `usuarios`
  ADD CONSTRAINT `fk_usuario_rol` FOREIGN KEY (`rol_id`) REFERENCES `rol_usuarios` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
