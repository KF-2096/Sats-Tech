CREATE TABLE `bg_task_last_run` (
  `id` int NOT NULL AUTO_INCREMENT,
  `last_run` datetime NOT NULL,
  `task_name` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


INSERT INTO `sats_tech`.`bg_task_last_run` (`id`, `last_run`, `task_name`) VALUES ('1', '2021-01-01 00:00:00', 'REMINDER_JOB');