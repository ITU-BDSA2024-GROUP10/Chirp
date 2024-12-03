function updateWindowWidth() {
    document.documentElement.style.setProperty('--window-width', `${window.innerWidth}px`);
}

// Run the function once when the page loads
updateWindowWidth();

document.addEventListener('DOMContentLoaded', function() {
    updateWindowWidth();
    window.addEventListener('resize', updateWindowWidth);
});