document.addEventListener('DOMContentLoaded', function () {
    const newsForm = document.getElementById('newsForm');
    const newsList = document.getElementById('newsList');

    // بارگذاری اخبار هنگام لود صفحه
    loadNews();

    // ثبت خبر جدید
    newsForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const formData = {
            title: document.getElementById('title').value,
            content: document.getElementById('content').value,
            image: document.getElementById('image').value,
            category: document.getElementById('category').value,
            date: new Date().toISOString()
        };

        fetch('/api/news', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData)
        })
            .then(response => response.json())
            .then(data => {
                alert('خبر با موفقیت ثبت شد!');
                newsForm.reset();
                loadNews();
            })
            .catch(error => {
                console.error('Error:', error);
                alert('خطا در ثبت خبر!');
            });
    });

    // تابع برای بارگذاری اخبار
    function loadNews() {
        fetch('/api/news')
            .then(response => response.json())
            .then(data => {
                newsList.innerHTML = '';
                if (data.length === 0) {
                    newsList.innerHTML = '<p class="text-gray-500">هیچ خبری ثبت نشده است.</p>';
                    return;
                }

                data.forEach(news => {
                    const newsItem = document.createElement('div');
                    newsItem.className = 'border border-gray-200 rounded-lg p-4';
                    newsItem.innerHTML = `
                        <div class="flex justify-between items-start">
                            <div>
                                <h3 class="font-bold text-lg">${news.title}</h3>
                                <p class="text-sm text-gray-500 mb-2">${news.category} - ${new Date(news.date).toLocaleString('fa-IR')}</p>
                            </div>
                            <button onclick="deleteNews('${news.id}')" class="text-red-500 hover:text-red-700">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                        <p class="text-gray-700 mt-2">${news.content}</p>
                        ${news.image ? `<img src="${news.image}" alt="${news.title}" class="mt-3 rounded-lg w-full max-w-xs">` : ''}
                    `;
                    newsList.appendChild(newsItem);
                });
            });
    }

    // تابع برای حذف خبر (به صورت گلوبال تعریف می‌شود)
    window.deleteNews = function (id) {
        if (confirm('آیا از حذف این خبر مطمئن هستید؟')) {
            fetch(`/api/news/${id}`, {
                method: 'DELETE'
            })
                .then(response => {
                    if (response.ok) {
                        loadNews();
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert('خطا در حذف خبر!');
                });
        }
    };
});