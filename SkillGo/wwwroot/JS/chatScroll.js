export function bottom(el) {
    if (!el) return;
    el.scrollTop = el.scrollHeight;
}