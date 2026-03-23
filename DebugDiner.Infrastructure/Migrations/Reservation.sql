DROP TABLE IF EXISTS `reservation`;

CREATE TABLE `reservation`
(
    `id`         INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `user_id`    INTEGER NOT NULL,
    `table_id`   INTEGER NOT NULL,
    `start_time` TEXT NOT NULL,
    `end_time`   TEXT NOT NULL,
    `guests`     INTEGER NOT NULL,
    `status`     TEXT NOT NULL CHECK(`status` IN ('Pending', 'Confirmed', 'Ongoing', 'Cancelled', 'Completed')),
    `created_at` TEXT NOT NULL default now(),
    `updated_at` TEXT NULL,
    FOREIGN KEY (`user_id`) REFERENCES `user`(`id`),
    FOREIGN KEY (`table_id`) REFERENCES `table`(`id`)
);

INSERT INTO `reservation` (`user_id`, `table_id`, `start_time`, `end_time`, `guests`, `status`, `created_at`, `updated_at`)
VALUES (1, 1, '2026-03-24 19:00:00', '2026-03-24 21:00:00', 2, 'Confirmed');