-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 06, 2025 at 08:34 PM
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
  `creado_por` bigint(20) UNSIGNED NOT NULL,
  `creado_at` datetime NOT NULL DEFAULT current_timestamp(),
  `finalizado_por` bigint(20) UNSIGNED DEFAULT NULL,
  `finalizado_at` datetime DEFAULT NULL,
  `eliminado_por` bigint(20) UNSIGNED DEFAULT NULL,
  `fecha_eliminacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `contratos`
--

INSERT INTO `contratos` (`id`, `inmueble_id`, `inquilino_id`, `fecha_inicio`, `fecha_fin_original`, `fecha_fin_efectiva`, `monto_mensual`, `creado_por`, `creado_at`, `finalizado_por`, `finalizado_at`, `eliminado_por`, `fecha_eliminacion`) VALUES
(12, 14, 25, '2025-10-01', '2027-09-01', NULL, 120000.00, 1, '2025-09-22 15:35:46', NULL, NULL, NULL, NULL),
(13, 15, 26, '2025-11-01', '2027-10-01', NULL, 135000.00, 1, '2025-09-22 15:35:46', NULL, NULL, NULL, NULL),
(14, 16, 27, '2025-12-01', '2027-11-01', NULL, 110000.00, 1, '2025-09-22 15:35:46', NULL, NULL, NULL, NULL),
(15, 17, 28, '2026-01-01', '2027-12-01', NULL, 140000.00, 1, '2025-09-22 15:35:46', NULL, NULL, NULL, NULL),
(16, 18, 29, '2026-02-01', '2028-01-01', NULL, 125000.00, 1, '2025-09-22 15:35:46', NULL, NULL, NULL, NULL),
(17, 19, 30, '2026-03-01', '2028-02-01', '2025-10-02', 150000.00, 1, '2025-09-22 15:35:46', 21, '2025-10-02 15:35:46', NULL, NULL),
(18, 20, 31, '2026-04-01', '2028-03-01', NULL, 115000.00, 1, '2025-09-22 15:35:46', NULL, NULL, NULL, NULL),
(19, 21, 32, '2026-05-01', '2028-04-01', '2025-10-02', 160000.00, 1, '2025-09-22 15:35:46', 1, '2025-10-02 15:35:46', NULL, NULL),
(20, 22, 33, '2026-06-01', '2028-05-01', '2025-09-23', 130000.00, 1, '2025-09-22 15:35:46', 1, '2025-09-23 18:19:37', 1, '2025-09-25 18:19:37'),
(21, 23, 34, '2026-07-01', '2028-06-01', NULL, 145000.00, 1, '2025-09-22 15:35:46', NULL, NULL, NULL, NULL),
(31, 14, 25, '2021-10-01', '2023-09-01', NULL, 120000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(32, 15, 26, '2021-11-01', '2023-10-01', NULL, 135000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(33, 16, 27, '2021-12-01', '2023-11-01', NULL, 110000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(34, 17, 28, '2022-01-01', '2023-12-01', NULL, 140000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(35, 18, 29, '2022-02-01', '2024-01-01', NULL, 125000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(36, 19, 30, '2022-03-01', '2025-12-23', NULL, 150000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(37, 20, 31, '2022-04-01', '2024-03-01', NULL, 115000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(38, 21, 32, '2022-05-01', '2025-11-13', NULL, 160000.00, 1, '2021-09-22 15:35:46', NULL, NULL, NULL, NULL),
(39, 22, 27, '2025-09-25', '2026-01-25', '2025-09-24', 700000.00, 1, '2025-09-24 22:32:58', 1, '2025-09-25 23:32:58', 1, '2025-09-26 02:01:59'),
(40, 15, 26, '2027-11-11', '2028-11-11', NULL, 148500.00, 1, '2025-09-25 18:23:01', 1, NULL, NULL, NULL),
(41, 23, 26, '2025-09-30', '2026-03-30', NULL, 80000.00, 1, '2025-09-26 18:44:00', NULL, NULL, NULL, NULL),
(42, 20, 25, '2025-10-22', '2026-03-22', NULL, 90000.00, 22, '2025-10-02 11:44:24', NULL, NULL, 1, '2025-10-03 23:36:11');

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

--
-- Dumping data for table `imagenes`
--

INSERT INTO `imagenes` (`id`, `inmueble_id`, `url`, `created_at`) VALUES
(10, 14, '/Uploads/14/14_67592a70-3609-412f-9a3d-47e00b66db73.png', '2025-10-03 23:16:17'),
(11, 14, '/Uploads/14/14_0c962dd2-0ee6-441e-b9c8-4317050f1595.png', '2025-10-03 23:16:17'),
(12, 14, '/Uploads/14/14_1cb0f614-8f22-4571-9dcf-11a85ea1f455.png', '2025-10-03 23:16:17'),
(13, 14, '/Uploads/14/14_9ecc589f-e1ed-4cbe-ba74-a1d5abd1ba54.png', '2025-10-03 23:16:17');

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
(14, 26, 1, 'RESIDENCIAL', 3, 'Av. Siempre Viva 742', -34.603722, -58.381592, 150000.00, 0, 'Casa familiar con jardín amplio.', '/Uploads/14.png', '2025-09-22 11:16:26', '2025-10-04 02:15:56', NULL),
(15, 27, 2, 'RESIDENCIAL', 2, 'Calle Falsa 123, Piso 4 Dto B', -34.615803, -58.433298, 85000.00, 0, 'Departamento luminoso en excelente ubicación.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(16, 28, 3, 'COMERCIAL', 1, 'Av. Corrientes 3500, Local 5', -34.603932, -58.410146, 220000.00, 0, 'Local comercial a la calle con gran vidriera.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(17, 29, 1, 'RESIDENCIAL', 4, 'San Martín 450', -34.598900, -58.377300, 180000.00, 0, 'Casa de dos plantas con patio trasero.', NULL, '2025-09-22 11:16:26', '2025-09-22 11:16:26', NULL),
(18, 30, 2, 'RESIDENCIAL', 1, 'Belgrano 1200, Piso 3', -34.620500, -58.385700, 70000.00, 1, 'Monoambiente ideal para estudiante.', NULL, '2025-09-22 11:16:26', '2025-10-04 19:13:54', NULL),
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
(34, '28765432', 'Cabrera', 'Rodrigo', '+54 9 11 2345-6789', 'rodrigo.cabrera@example.com', NULL),
(37, '45164785', 'Martinez', 'Mario', '2664759416', 'Jose@gmail.com', NULL),
(38, '47156235', 'Solari', 'Gaston', '2664784618', 'SolariGaston@gmail.com', NULL);

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
(14, 12, 1, '2025-09-22', 'Adelanto Octubre', 120000.00, 'Anulado', 1, '2025-09-22 19:36:42', 1, '2025-09-26 01:44:19'),
(15, 12, 2, '2025-09-22', 'completo', 120000.00, 'Anulado', 1, '2025-09-22 19:37:04', 1, '2025-09-26 01:44:00'),
(16, 20, 1, '2025-09-23', 'Alquiler mes Septiembre', 130000.00, 'Pagado', 1, '2025-09-23 13:43:03', NULL, NULL),
(17, 20, 2, '2025-09-23', 'Adelanto Octubre', 130000.00, 'Anulado', 1, '2025-09-23 13:45:12', 1, '2025-09-23 14:22:39'),
(18, 20, 3, '2025-09-23', 'completo', 2990000.00, 'Anulado', 1, '2025-09-23 13:45:38', 1, '2025-09-26 01:32:06'),
(19, 20, 4, '2025-09-23', 'completo', 130000.00, 'Pagado', 1, '2025-09-23 14:23:08', NULL, NULL),
(20, 39, 1, '2025-09-24', 'Alquiler mes Septiembre', 700000.00, 'Anulado', 1, '2025-09-24 22:33:22', 1, '2025-09-26 01:24:24'),
(21, 39, 2, '2025-09-24', 'Adelanto Octubre', 700000.00, 'Anulado', 1, '2025-09-24 22:33:31', 1, '2025-09-26 01:37:54'),
(22, 39, 3, '2025-09-24', 'completo', 1400000.00, 'Anulado', 1, '2025-09-24 22:33:37', 1, '2025-09-26 02:00:51'),
(23, 40, 1, '2025-09-26', 'Adelanto Octubre', 148500.00, 'Pagado', 1, '2025-09-26 01:47:35', NULL, NULL),
(24, 40, 2, '2025-09-26', 'Alquiler mes Septiembre', 148500.00, 'Anulado', 1, '2025-09-26 01:47:41', 1, '2025-09-26 01:59:11'),
(25, 40, 3, '2025-09-26', 'Adelanto Octubre', 148500.00, 'Anulado', 1, '2025-09-26 01:47:45', 1, '2025-09-26 01:50:53'),
(26, 40, 4, '2025-09-26', 'Alquiler', 148500.00, 'Anulado', 1, '2025-09-26 01:47:54', 1, '2025-09-26 01:49:20'),
(27, 40, 5, '2025-09-26', 'Alquiler', 148500.00, 'Pagado', 1, '2025-09-26 01:48:06', NULL, NULL),
(28, 39, 4, '2025-09-26', 'Alquiler', 700000.00, 'Pagado', 1, '2025-09-26 02:01:08', NULL, NULL),
(29, 39, 5, '2025-09-26', 'Alquiler', 700000.00, 'Pagado', 1, '2025-09-26 02:01:12', NULL, NULL),
(30, 39, 6, '2025-09-26', 'Alquiler', 700000.00, 'Pagado', 1, '2025-09-26 02:01:15', NULL, NULL),
(31, 39, 7, '2025-09-26', 'Alquiler', 700000.00, 'Pagado', 1, '2025-09-26 02:01:19', NULL, NULL),
(32, 39, 8, '2025-09-26', 'Alquiler', 700000.00, 'Pagado', 1, '2025-09-26 02:01:23', NULL, NULL),
(33, 19, 1, '2025-10-04', 'Alquiler', 160000.00, 'Pagado', 1, '2025-10-04 02:19:51', NULL, NULL),
(34, 19, 2, '2025-10-04', 'Alquiler', 3840000.00, 'Pagado', 1, '2025-10-04 02:20:12', NULL, NULL);

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
(37, '47156235', 'Solari', 'Gaston', '2664784618', 'SolariGaston@gmail.com', NULL),
(38, '27456321', 'Pereyra', 'Julián', '+54 9 11 5566-7788', 'julian.pereyra@example.com', NULL);

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
(1, 'Administrador', 'Acceso completo al sistema', '2025-09-18 16:01:50', '2025-09-30 17:25:45'),
(2, 'Empleado', 'Empleado interno con acceso a funciones operativas', '2025-09-18 16:01:50', '2025-09-18 16:01:50');

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
(3, 'Local', 'Comercial a la calle', 1, '2025-09-08 18:32:37', '2025-09-08 18:32:37', NULL),
(4, 'Vivienda 2', 'Vivienda Barrial', 1, '2025-10-02 01:24:08', '2025-10-02 07:57:07', NULL),
(5, 'Vivienda 3', 'nada', 1, '2025-10-02 11:06:27', '2025-10-02 08:19:50', '2025-10-02 08:19:50');

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
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `fecha_eliminacion` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `usuarios`
--

INSERT INTO `usuarios` (`id`, `email`, `password_hash`, `nombre`, `apellido`, `telefono`, `avatar_url`, `rol_id`, `is_active`, `created_at`, `updated_at`, `fecha_eliminacion`) VALUES
(1, 'admin@gmail.com', 'ps0p9TLGtwIktVenb12Gxg==:+3y1LsJGBQwmbqvKG/w2TZlTW7nd92xTNonS5CivjrE=', 'Juan', 'Pérez', '+54 9 11 2345-6789', '/uploads/avatars/avatar_1_638948307589272209.jpeg', 1, 1, '2025-09-18 16:07:01', '2025-10-04 00:51:30', NULL),
(21, 'admin1@gmail.com', 'G8t3vhHA39smHvr05Gf92A==:zu/n74UxdKXA/a+tcD0UdqBf/PhbWgJ8rwk4VqLMmn8=', 'Maria', 'Veliz', '2664789413', NULL, 1, 1, '2025-09-27 12:45:04', '2025-10-02 17:37:22', NULL),
(22, 'empleado@gmail.com', 'gReuPl2WSamq4qImLL+zOg==:YCVopDuS1EnL7eIghhYagRMZ1dVpkvUIHvpGli3Ffa4=', 'Santiago', 'Ramirez', '2664789461', NULL, 2, 1, '2025-09-30 20:28:21', '2025-10-06 18:31:27', NULL),
(23, 'empleado2@gmail.com', 'RgvT7KcL63AahvRiJx889g==:Gp5Unt7hSZSwl9bw5zAggd7UdEKSmFLWqpILvmfdMZg=', 'Patricio', 'Jorgue', '2664789413', NULL, 2, 1, '2025-10-02 20:40:29', '2025-10-02 20:40:29', NULL);

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
  ADD KEY `ix_contratos_fechas` (`fecha_inicio`,`fecha_fin_original`),
  ADD KEY `ix_contratos_creado_por` (`creado_por`),
  ADD KEY `eliminado_por` (`eliminado_por`);

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
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=43;

--
-- AUTO_INCREMENT for table `imagenes`
--
ALTER TABLE `imagenes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=39;

--
-- AUTO_INCREMENT for table `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;

--
-- AUTO_INCREMENT for table `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=39;

--
-- AUTO_INCREMENT for table `rol_usuarios`
--
ALTER TABLE `rol_usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `tipos_inmueble`
--
ALTER TABLE `tipos_inmueble`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `fk_contratos_creado_por` FOREIGN KEY (`creado_por`) REFERENCES `usuarios` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_eliminado_por` FOREIGN KEY (`eliminado_por`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_finalizado_por` FOREIGN KEY (`finalizado_por`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_inmueble` FOREIGN KEY (`inmueble_id`) REFERENCES `inmuebles` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_inquilino` FOREIGN KEY (`inquilino_id`) REFERENCES `inquilinos` (`id`) ON UPDATE CASCADE;

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
  ADD CONSTRAINT `fk_pagos_contrato` FOREIGN KEY (`contrato_id`) REFERENCES `contratos` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
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
