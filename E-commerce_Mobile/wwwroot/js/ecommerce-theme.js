/**
 * E-Commerce Mobile Theme JavaScript
 * Enhanced functionality for mobile-first e-commerce experience
 */

document.addEventListener('DOMContentLoaded', function() {
    // Initialize theme functionality
    initializeTheme();
    initializeSidebarToggle();
    initializeAnimations();
    initializeSearchEnhancements();
    initializeStatsCounter();
    initializeTooltips();
});

/**
 * Initialize theme functionality
 */
function initializeTheme() {
    // Add active class to current navigation item
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.remove('collapsed');
            link.classList.add('active');
        }
    });

    // Add smooth transitions to all elements
    document.body.style.transition = 'all 0.3s ease';
}

/**
 * Enhanced sidebar toggle functionality
 */
function initializeSidebarToggle() {
    const toggleBtn = document.querySelector('.toggle-sidebar-btn');
    const sidebar = document.querySelector('.sidebar');
    const body = document.body;
    
    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener('click', function() {
            body.classList.toggle('toggle-sidebar');
            
            // Add animation effect
            toggleBtn.style.transform = body.classList.contains('toggle-sidebar') 
                ? 'rotate(180deg)' 
                : 'rotate(0deg)';
            
            // For mobile devices, handle overlay
            if (window.innerWidth <= 576) {
                sidebar.classList.toggle('show');
                
                // Add overlay for mobile
                if (sidebar.classList.contains('show')) {
                    createMobileOverlay();
                } else {
                    removeMobileOverlay();
                }
            }
        });
        
        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', function(e) {
            if (window.innerWidth <= 576 && 
                sidebar.classList.contains('show') && 
                !sidebar.contains(e.target) && 
                !toggleBtn.contains(e.target)) {
                sidebar.classList.remove('show');
                removeMobileOverlay();
            }
        });
    }
}

/**
 * Create mobile overlay
 */
function createMobileOverlay() {
    if (!document.querySelector('.mobile-overlay')) {
        const overlay = document.createElement('div');
        overlay.className = 'mobile-overlay';
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            z-index: 999;
            backdrop-filter: blur(4px);
            animation: fadeIn 0.3s ease;
        `;
        
        overlay.addEventListener('click', function() {
            document.querySelector('.sidebar').classList.remove('show');
            removeMobileOverlay();
        });
        
        document.body.appendChild(overlay);
    }
}

/**
 * Remove mobile overlay
 */
function removeMobileOverlay() {
    const overlay = document.querySelector('.mobile-overlay');
    if (overlay) {
        overlay.style.animation = 'fadeOut 0.3s ease';
        setTimeout(() => overlay.remove(), 300);
    }
}

/**
 * Initialize animations
 */
function initializeAnimations() {
    // Intersection Observer for fade-in animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);
    
    // Observe all elements with fade-in-up class
    document.querySelectorAll('.fade-in-up').forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(30px)';
        el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(el);
    });
    
    // Add hover effects to cards
    document.querySelectorAll('.card, .stats-card, .product-card').forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-5px)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });
}

/**
 * Enhanced search functionality
 */
function initializeSearchEnhancements() {
    const searchInput = document.querySelector('.search-bar input[type="text"]');
    const searchButton = document.querySelector('.search-bar button');
    
    if (searchInput) {
        // Add search suggestions (placeholder for future enhancement)
        searchInput.addEventListener('focus', function() {
            this.parentElement.style.transform = 'translateY(-2px)';
            this.parentElement.style.boxShadow = '0 8px 25px rgba(99, 102, 241, 0.15)';
        });
        
        searchInput.addEventListener('blur', function() {
            this.parentElement.style.transform = 'translateY(0)';
            this.parentElement.style.boxShadow = '';
        });
        
        // Real-time search (placeholder for future implementation)
        searchInput.addEventListener('input', function() {
            const query = this.value.trim();
            if (query.length > 2) {
                // Implement search logic here
                console.log('Searching for:', query);
            }
        });
    }
}

/**
 * Animated counter for stats
 */
function initializeStatsCounter() {
    const statsNumbers = document.querySelectorAll('.stats-number');
    
    const animateCounter = (element) => {
        const target = element.textContent.replace(/[^\d]/g, '');
        const prefix = element.textContent.replace(/[\d,]/g, '');
        const duration = 1000;
        const increment = target / (duration / 16);
        let current = 0;
        
        const updateCounter = () => {
            current += increment;
            if (current < target) {
                element.textContent = prefix + Math.floor(current).toLocaleString();
                requestAnimationFrame(updateCounter);
            } else {
                element.textContent = prefix + parseInt(target).toLocaleString();
            }
        };
        
        updateCounter();
    };
    
    // Animate counters when they come into view
    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && !entry.target.dataset.animated) {
                animateCounter(entry.target);
                entry.target.dataset.animated = 'true';
            }
        });
    }, { threshold: 0.5 });
    
    statsNumbers.forEach(el => counterObserver.observe(el));
}

/**
 * Initialize tooltips
 */
function initializeTooltips() {
    // Add tooltips to buttons and links
    document.querySelectorAll('[title]').forEach(element => {
        element.addEventListener('mouseenter', function(e) {
            if (this.title) {
                createTooltip(this, this.title);
            }
        });
        
        element.addEventListener('mouseleave', function() {
            removeTooltip();
        });
    });
}

/**
 * Create tooltip
 */
function createTooltip(element, text) {
    removeTooltip(); // Remove existing tooltip
    
    const tooltip = document.createElement('div');
    tooltip.className = 'custom-tooltip';
    tooltip.textContent = text;
    tooltip.style.cssText = `
        position: absolute;
        background: var(--gray-800);
        color: white;
        padding: 8px 12px;
        border-radius: 6px;
        font-size: 12px;
        white-space: nowrap;
        z-index: 9999;
        pointer-events: none;
        opacity: 0;
        transform: translateY(10px);
        transition: opacity 0.3s ease, transform 0.3s ease;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    `;
    
    document.body.appendChild(tooltip);
    
    const rect = element.getBoundingClientRect();
    tooltip.style.left = rect.left + (rect.width / 2) - (tooltip.offsetWidth / 2) + 'px';
    tooltip.style.top = rect.top - tooltip.offsetHeight - 8 + 'px';
    
    // Animate tooltip
    requestAnimationFrame(() => {
        tooltip.style.opacity = '1';
        tooltip.style.transform = 'translateY(0)';
    });
}

/**
 * Remove tooltip
 */
function removeTooltip() {
    const tooltip = document.querySelector('.custom-tooltip');
    if (tooltip) {
        tooltip.style.opacity = '0';
        tooltip.style.transform = 'translateY(10px)';
        setTimeout(() => tooltip.remove(), 300);
    }
}

/**
 * Handle window resize
 */
window.addEventListener('resize', function() {
    const sidebar = document.querySelector('.sidebar');
    const body = document.body;
    
    // Reset sidebar state on resize
    if (window.innerWidth > 576) {
        sidebar.classList.remove('show');
        removeMobileOverlay();
    }
    
    // Adjust main content padding
    if (window.innerWidth <= 768) {
        body.classList.add('mobile-view');
    } else {
        body.classList.remove('mobile-view');
    }
});

/**
 * Smooth scroll for anchor links
 */
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

/**
 * Add loading states to buttons
 */
document.querySelectorAll('.btn').forEach(button => {
    button.addEventListener('click', function(e) {
        // Add ripple effect
        const ripple = document.createElement('span');
        const rect = this.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;
        
        ripple.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            left: ${x}px;
            top: ${y}px;
            background: rgba(255, 255, 255, 0.6);
            border-radius: 50%;
            transform: scale(0);
            animation: ripple 0.6s linear;
            pointer-events: none;
        `;
        
        this.appendChild(ripple);
        
        setTimeout(() => ripple.remove(), 600);
    });
});

// Add CSS animation for ripple effect
const style = document.createElement('style');
style.textContent = `
    @keyframes ripple {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
    
    @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
    }
    
    @keyframes fadeOut {
        from { opacity: 1; }
        to { opacity: 0; }
    }
    
    .btn {
        position: relative;
        overflow: hidden;
    }
    
    .mobile-view .sidebar {
        position: fixed;
        left: -100%;
        transition: left 0.3s ease;
    }
    
    .mobile-view .sidebar.show {
        left: 0;
    }
`;

document.head.appendChild(style);
