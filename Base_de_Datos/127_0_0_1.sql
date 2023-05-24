-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 24-05-2023 a las 05:21:45
-- Versión del servidor: 10.4.27-MariaDB
-- Versión de PHP: 8.0.25

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `macetatwo`
--
CREATE DATABASE IF NOT EXISTS `macetatwo` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `macetatwo`;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `registro`
--
-- Creación: 23-05-2023 a las 03:56:31
--

CREATE TABLE `registro` (
  `id_Registro` int(11) NOT NULL,
  `id_Usuario` int(11) NOT NULL,
  `sensor_Uno` varchar(50) NOT NULL,
  `valor_Uno` float NOT NULL,
  `op_Uno` varchar(50) NOT NULL,
  `sensor_Dos` varchar(50) NOT NULL,
  `valor_Dos` float NOT NULL,
  `op_Dos` varchar(50) NOT NULL,
  `fec_hora` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Volcado de datos para la tabla `registro`
--

INSERT INTO `registro` (`id_Registro`, `id_Usuario`, `sensor_Uno`, `valor_Uno`, `op_Uno`, `sensor_Dos`, `valor_Dos`, `op_Dos`, `fec_hora`) VALUES
(1, 1, 'sensor uno', 1.99, 'ta bien', '1.99', 99.99, 'saquenme de aqui!!!!', '2023-05-22 22:11:04');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--
-- Creación: 23-05-2023 a las 04:04:38
--

CREATE TABLE `usuarios` (
  `id_Usuario` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `contra` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`id_Usuario`, `nombre`, `contra`) VALUES
(1, 'admin', '123');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `registro`
--
ALTER TABLE `registro`
  ADD PRIMARY KEY (`id_Registro`),
  ADD KEY `registro_idfk_1` (`id_Usuario`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id_Usuario`),
  ADD UNIQUE KEY `nombre` (`nombre`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `registro`
--
ALTER TABLE `registro`
  MODIFY `id_Registro` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id_Usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `registro`
--
ALTER TABLE `registro`
  ADD CONSTRAINT `registro_idfk_1` FOREIGN KEY (`id_Usuario`) REFERENCES `usuarios` (`id_Usuario`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
