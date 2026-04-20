CREATE TABLE IF NOT EXISTS `reservation_arrangement`
(
    `id`          INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `reservation_id` INTEGER NOT NULL,
    `arrangement_id` INTEGER NOT NULL,
    FOREIGN KEY (`reservation_id`) REFERENCES `reservation`(`id`),
    FOREIGN KEY (`arrangement_id`) REFERENCES `arrangement`(`id`)
);

INSERT INTO `reservation_arrangement` (`arrangement_id`, `reservation_id`) VALUES (1, 1);
