CREATE IF NOT EXISTS TABLE `dish`
(
    `id`           INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    `name`         TEXT NOT NULL,
    `price`        REAL NOT NULL,
    `description`  TEXT NOT NULL,
    `category`     TEXT NOT NULL CHECK(`category` IN ('Meat', 'Fish', 'Vegetarian', 'Vegan')),
    `allergen_info` TEXT NOT NULL,
    `created_at`   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at`   TEXT NULL
);

INSERT INTO `dish` (`name`, `price`, `description`, `category`, `allergen_info`, `created_at`, `updated_at`)
VALUES ('Caesar Salad', 8.50, 'Romaine lettuce, parmesan, croutons, caesar dressing', 'Vegetarian', 'gluten, dairy, egg', DATETIME('now'), DATETIME('now'));

DROP TRIGGER IF EXISTS trg_dish_updated_at;
CREATE TRIGGER trg_dish_updated_at
AFTER UPDATE ON `dish`
FOR EACH ROW
BEGIN
    UPDATE `dish` SET updated_at = DATETIME('now') WHERE id = NEW.id;
END;
