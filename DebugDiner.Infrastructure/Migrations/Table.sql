DROP TABLE IF EXISTS `table`;

CREATE TABLE `table`
(
    `id`         INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `capacity`   INTEGER NOT NULL,
    `type`       TEXT NOT NULL CHECK(`type` IN ('TwoPerson', 'FourPerson', 'SixPerson', 'Bar')),
    `created_at` TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` TEXT NULL
);

INSERT INTO `table` (`capacity`, `type`, `created_at`, `updated_at`) VALUES (4, 'Bar', DATETIME('now'), DATETIME('now'));

DROP TRIGGER IF EXISTS trg_table_updated_at;
CREATE TRIGGER trg_table_updated_at
AFTER UPDATE ON `table`
FOR EACH ROW
BEGIN
    UPDATE `table` SET updated_at = DATETIME('now') WHERE id = NEW.id;
END;
