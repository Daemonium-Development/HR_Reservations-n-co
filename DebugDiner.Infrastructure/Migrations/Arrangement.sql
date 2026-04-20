CREATE TABLE IF NOT EXISTS `arrangement`
(
    `id`         INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `name`       TEXT NOT NULL,
    `base_price` REAL NOT NULL,
    `type`       TEXT NOT NULL CHECK(`type` IN ('TwoCourse', 'ThreeCourse', 'FourCourse', 'Wine')),
    `created_at` TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` TEXT NULL
);

INSERT INTO `arrangement` (`name`, `base_price`, `type`, `created_at`, `updated_at`)
VALUES ('Classic Dinner', '29.99', 'TwoCourse', DATETIME('now'), DATETIME('now'));

DROP TRIGGER IF EXISTS trg_arrangement_updated_at;
CREATE TRIGGER trg_arrangement_updated_at
AFTER UPDATE ON `arrangement`
FOR EACH ROW
BEGIN
    UPDATE `arrangement` SET updated_at = DATETIME('now') WHERE id = NEW.id;
END;
