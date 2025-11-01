const express = require('express');
const path = require('path');
const app = express();
const PORT = process.env.PORT || 3000;

// اصلاح مسیر به wwwroot
app.use(express.static(path.join(__dirname, 'wwwroot')));

// بقیه کدها...

// Middleware
app.use(morgan('dev'));
app.use(express.json());
app.use(express.static(path.join(__dirname, 'public')));

// Mini APIs
app.use('/api/news', require('./api/news'));

// Route for admin panel
app.get('/admin', (req, res) => {
    res.sendFile(path.join(__dirname, 'public', 'index.html'));
});

// Start server
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});