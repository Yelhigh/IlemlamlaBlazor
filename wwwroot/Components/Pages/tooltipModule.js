export function showTooltip(x, y, text) {
    const tooltip = document.getElementById('tooltip');
    if (tooltip) {
        tooltip.style.display = 'block';
        tooltip.style.left = `${x + 10}px`;
        tooltip.style.top = `${y + 10}px`;
        tooltip.textContent = text;
    }
}

export function hideTooltip() {
    const tooltip = document.getElementById('tooltip');
    if (tooltip) {
        tooltip.style.display = 'none';
    }
} 