DROP TABLE IF EXISTS `reservation_arrangement`;

CREATE TABLE `reservation_arrangement`
(
    `id`          INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `reservation` INTEGER NOT NULL,
    `arrangement` INTEGER NOT NULL,
    FOREIGN KEY (`reservation`) REFERENCES `reservation`(`id`),
    FOREIGN KEY (`arrangement`) REFERENCES `arrangement`(`id`)
);

INSERT INTO `reservation_arrangement` (`reservation`, `arrangement`)
VALUES (1, 1);