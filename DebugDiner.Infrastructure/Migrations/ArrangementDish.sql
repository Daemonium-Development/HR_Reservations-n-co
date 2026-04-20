CREATE TABLE IF NOT EXISTS `arrangement_dish`
(
    `id`          INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `dish_id`        INTEGER NOT NULL,
    `arrangement_id` INTEGER NOT NULL,
    FOREIGN KEY (`dish_id`) REFERENCES `dish`(`id`),
    FOREIGN KEY (`arrangement_id`) REFERENCES `arrangement`(`id`)
);

INSERT INTO `arrangement_dish` (`dish_id`, `arrangement_id`)
VALUES (1, 1);
