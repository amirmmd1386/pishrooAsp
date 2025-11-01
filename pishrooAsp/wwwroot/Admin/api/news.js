const express = require('express');
const fs = require('fs');
const path = require('path');
const router = express.Router();

const dataPath = path.join(__dirname, '../data/news.json');

// ایجاد فایل داده اگر وجود نداشته باشد
if (!fs.existsSync(dataPath)) {
    fs.writeFileSync(dataPath, '[]', 'utf8');
}

// دریافت همه اخبار
router.get('/', (req, res) => {
    fs.readFile(dataPath, 'utf8', (err, data) => {
        if (err) {
            return res.status(500).json({ error: 'خطا در خواندن داده‌ها' });
        }
        res.json(JSON.parse(data));
    });
});

// افزودن خبر جدید
router.post('/', (req, res) => {
    fs.readFile(dataPath, 'utf8', (err, data) => {
        if (err) {
            return res.status(500).json({ error: 'خطا در خواندن داده‌ها' });
        }

        const news = JSON.parse(data);
        const newNews = {
            id: Date.now().toString(),
            ...req.body
        };

        news.push(newNews);

        fs.writeFile(dataPath, JSON.stringify(news, null, 2), 'utf8', (err) => {
            if (err) {
                return res.status(500).json({ error: 'خطا در ذخیره داده‌ها' });
            }
            res.status(201).json(newNews);
        });
    });
});

// حذف خبر
router.delete('/:id', (req, res) => {
    fs.readFile(dataPath, 'utf8', (err, data) => {
        if (err) {
            return res.status(500).json({ error: 'خطا در خواندن داده‌ها' });
        }

        let news = JSON.parse(data);
        const initialLength = news.length;
        news = news.filter(item => item.id !== req.params.id);

        if (news.length === initialLength) {
            return res.status(404).json({ error: 'خبر یافت نشد' });
        }

        fs.writeFile(dataPath, JSON.stringify(news, null, 2), 'utf8', (err) => {
            if (err) {
                return res.status(500).json({ error: 'خطا در ذخیره داده‌ها' });
            }
            res.status(204).end();
        });
    });
});

module.exports = router;