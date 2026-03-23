DROP TABLE IF EXISTS `arrangement_dish`;

CREATE TABLE `arrangement_dish`
(
    `id`          INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `dish`        INTEGER NOT NULL,
    `arrangement` INTEGER NOT NULL,
    FOREIGN KEY (`dish`) REFERENCES `dish`(`id`),
    FOREIGN KEY (`arrangement`) REFERENCES `arrangement`(`id`)
);

INSERT INTO `arrangement_dish` (`dish`, `arrangement`)
VALUES (1, 1);