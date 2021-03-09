CREATE DATABASE  IF NOT EXISTS `sats_tech` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_bin */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `sats_tech`;
-- MySQL dump 10.13  Distrib 8.0.18, for Win64 (x86_64)
--
-- Host: localhost    Database: sats_tech
-- ------------------------------------------------------
-- Server version	8.0.18

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `customer`
--

DROP TABLE IF EXISTS `customer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) COLLATE utf8_bin NOT NULL,
  `mobile` char(10) COLLATE utf8_bin NOT NULL,
  `sid` varchar(45) COLLATE utf8_bin NOT NULL,
  `vc_number` varchar(45) COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `vc_number_UNIQUE` (`vc_number`),
  UNIQUE KEY `sid_UNIQUE` (`sid`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer`
--

LOCK TABLES `customer` WRITE;
/*!40000 ALTER TABLE `customer` DISABLE KEYS */;
INSERT INTO `customer` VALUES (1,'Cust 1','0773429310','3333333333','213123'),(2,'test1','123123123','132123123','23412312'),(4,'Cust 2','0743788732','565443','23243402');
/*!40000 ALTER TABLE `customer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reload`
--

DROP TABLE IF EXISTS `reload`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reload` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `customer_id` int(11) NOT NULL,
  `tx_date` datetime NOT NULL,
  `expiry_date` datetime DEFAULT NULL,
  `provider` varchar(45) COLLATE utf8_bin NOT NULL,
  `total` decimal(8,2) NOT NULL,
  `pack_desc` varchar(45) COLLATE utf8_bin NOT NULL,
  `pack_amount` decimal(8,2) NOT NULL DEFAULT '0.00',
  `addOn_desc` varchar(45) COLLATE utf8_bin DEFAULT NULL,
  `addOn_amount` decimal(8,2) DEFAULT '0.00',
  `extracharge_desc` varchar(45) COLLATE utf8_bin DEFAULT NULL,
  `extracharge_amount` decimal(8,2) DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `reload_customer_idx` (`customer_id`),
  CONSTRAINT `reload_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reload`
--

LOCK TABLES `reload` WRITE;
/*!40000 ALTER TABLE `reload` DISABLE KEYS */;
INSERT INTO `reload` VALUES (2,1,'2021-03-05 11:45:03',NULL,'D2H',600.00,'System.Windows.Controls.TextBox: Test pacl',100.00,'System.Windows.Controls.TextBox: test addon',200.00,'System.Windows.Controls.TextBox: test extra',300.00),(3,1,'2021-03-05 13:39:12',NULL,'D2H',100.00,'',100.00,'',0.00,'',0.00),(4,1,'2021-03-05 13:42:31',NULL,'D2H',100.00,'',100.00,'',0.00,'',0.00),(5,1,'2021-03-05 13:49:16',NULL,'D2H',122.00,'',122.00,'',0.00,'',0.00),(6,1,'2021-03-05 13:56:47','2021-03-12 00:00:00','D2H',123.00,'',123.00,'',0.00,'',0.00);
/*!40000 ALTER TABLE `reload` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sms_queue`
--

DROP TABLE IF EXISTS `sms_queue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sms_queue` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `mobile_number` varchar(45) COLLATE utf8_bin NOT NULL,
  `message` mediumtext COLLATE utf8_bin NOT NULL,
  `status` enum('PENDING','SENT','FAILED') COLLATE utf8_bin DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sms_queue`
--

LOCK TABLES `sms_queue` WRITE;
/*!40000 ALTER TABLE `sms_queue` DISABLE KEYS */;
INSERT INTO `sms_queue` VALUES (1,'0773429310','Dear customer\\nThank you for recharging through our system.\\nPlease keep the receiver switched on\\nHelp Line : 0768866972','SENT');
/*!40000 ALTER TABLE `sms_queue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'sats_tech'
--

--
-- Dumping routines for database 'sats_tech'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-03-09 14:24:19
