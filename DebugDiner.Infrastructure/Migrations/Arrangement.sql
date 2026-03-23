DROP TABLE IF EXISTS `arrangement`;

CREATE TABLE `arrangement`
(
    `id`         INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `name`       TEXT NOT NULL,
    `base_price` REAL NOT NULL,
    `type`       TEXT NOT NULL CHECK(`type` IN ('TwoCourse', 'ThreeCourse', 'FourCourse', 'Wine')),
    `created_at` TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` TEXT NULL
);

INSERT INTO `arrangement` (`name`, `base_price`, `type`, `created_at`, `updated_at`)
VALUES ('Classic Dinner', '29.99', 'TwoCourse');
