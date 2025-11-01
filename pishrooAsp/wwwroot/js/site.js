//document.getElementById('current-year').textContent = new Date().getFullYear();

// Mobile Menu Toggle
const mobileMenuButton = document.getElementById('mobile-menu-button');
const mobileMenu = document.getElementById('mobile-menu');
mobileMenuButton.addEventListener('click', () => {
    mobileMenu.classList.toggle('hidden');
});

// Language Switcher
const currentLangSpan = document.getElementById('current-lang');
const langLinks = document.querySelectorAll('[data-lang]');
langLinks.forEach(link => {
    link.addEventListener('click', (e) => {
        e.preventDefault();
        const selectedLang = e.target.dataset.lang;
        if (selectedLang === 'fa') {
            document.documentElement.setAttribute('lang', 'fa');
            document.documentElement.setAttribute('dir', 'rtl');
            currentLangSpan.textContent = 'فارسی';
            // You would typically update content here based on language
            // For example: updateTextContent('fa');
        } else if (selectedLang === 'en') {
            document.documentElement.setAttribute('lang', 'en');
            document.documentElement.setAttribute('dir', 'ltr');
            currentLangSpan.textContent = 'English';
            // For example: updateTextContent('en');
        }
        // Hide dropdown after selection (optional, but good UX)
        e.target.closest('.group').querySelector('div').classList.add('hidden');
    });
});

const slides = document.querySelectorAll(".slider-item");
const dots = document.querySelectorAll(".dot");
let currentSlide = 0;
let slideInterval;

function showSlide(index) {
    slides.forEach((slide, i) => {
        slide.classList.remove("active");
        dots[i].classList.remove("active-dot");
    });
    slides[index].classList.add("active");
    dots[index].classList.add("active-dot");
}

function nextSlide() {
    currentSlide = (currentSlide + 1) % slides.length;
    showSlide(currentSlide);
}

function prevSlide() {
    currentSlide = (currentSlide - 1 + slides.length) % slides.length;
    showSlide(currentSlide);
}

// Start autoplay
function startSlider() {
    slideInterval = setInterval(nextSlide, 5000); // تغییر اسلاید هر ۵ ثانیه
}

function stopSlider() {
    clearInterval(slideInterval);
}

// رویداد دکمه‌ها
document.getElementById("next-slide").addEventListener("click", () => {
    stopSlider();
    nextSlide();
    startSlider();
});

document.getElementById("prev-slide").addEventListener("click", () => {
    stopSlider();
    prevSlide();
    startSlider();
});

// رویداد کلیک روی دات‌ها
dots.forEach((dot, index) => {
    dot.addEventListener("click", () => {
        stopSlider();
        currentSlide = index;
        showSlide(currentSlide);
        startSlider();
    });
});

// شروع اسلایدر
startSlider();

// Smooth scroll for internal links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelector(this.getAttribute('href')).scrollIntoView({
            behavior: 'smooth'
        });
        // Close mobile menu after clicking a link
        if (!mobileMenu.classList.contains('hidden')) {
            mobileMenu.classList.add('hidden');
        }
    });
});
document.addEventListener('DOMContentLoaded', function () {
    const langButton = document.getElementById('langButton');
    const langDropdown = document.getElementById('langDropdown');
    const chevronIcon = document.getElementById('chevronIcon');

    langButton.addEventListener('click', function (e) {
        e.stopPropagation(); // جلوگیری از بسته شدن منو هنگام کلیک روی دکمه
        if (langDropdown.classList.contains('hidden')) {
            langDropdown.classList.remove('hidden');
            langDropdown.classList.remove('scale-y-0');
            langDropdown.classList.add('scale-y-100');
            chevronIcon.classList.add('rotate-180');
        } else {
            langDropdown.classList.add('hidden');
            langDropdown.classList.remove('scale-y-100');
            langDropdown.classList.add('scale-y-0');
            chevronIcon.classList.remove('rotate-180');
        }
    });

    // بستن منو اگر جایی بیرون منو کلیک شد
    document.addEventListener('click', function () {
        if (!langDropdown.classList.contains('hidden')) {
            langDropdown.classList.add('hidden');
            langDropdown.classList.remove('scale-y-100');
            langDropdown.classList.add('scale-y-0');
            chevronIcon.classList.remove('rotate-180');
        }
    });
});
class VisitLogger {
    static async logVisit(path = null) {
        try {
            const actualPath = path || window.location.pathname;

            await fetch('/api/visit/log', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    path: actualPath,
                    referrer: document.referrer,
                    screenResolution: `${screen.width}x${screen.height}`,
                    language: navigator.language
                })
            });
        } catch (error) {
            console.log('خطا در ثبت بازدید:', error);
        }
    }

    // ثبت بازدید وقتی صفحه لود شد
    static init() {
        document.addEventListener('DOMContentLoaded', function () {
            VisitLogger.logVisit();
        });

        // ثبت بازدید وقتی کاربر به صفحه دیگری می‌رود (برای SPAها)
        window.addEventListener('beforeunload', function () {
            VisitLogger.logVisit();
        });
    }
}

// راه‌اندازی
VisitLogger.init();