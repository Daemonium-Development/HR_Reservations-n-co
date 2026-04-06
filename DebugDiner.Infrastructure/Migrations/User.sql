CREATE IF NOT EXISTS TABLE `user`
(
    `id`            INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `name`          TEXT NOT NULL,
    `email`         TEXT NOT NULL,
    `password_hash` TEXT NOT NULL,
    `role`          TEXT NOT NULL CHECK(`role` IN ('Admin', 'Employee', 'Customer')),
    `created_at`    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at`    TEXT NULL
);

INSERT INTO `user` (`name`, `email`, `password_hash`, `role`, `created_at`, `updated_at`)
VALUES ('Soufian Manai', 'graviaRotterdam@gmail.com', '$2a$12$examplehashhere', 'Admin', DATETIME('now'), DATETIME('now'));

DROP TRIGGER IF EXISTS trg_user_updated_at;
CREATE TRIGGER trg_user_updated_at
AFTER UPDATE ON `user`
FOR EACH ROW
BEGIN
    UPDATE `user` SET updated_at = DATETIME('now') WHERE id = NEW.id;
END;
