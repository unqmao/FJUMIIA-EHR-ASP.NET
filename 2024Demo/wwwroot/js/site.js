function initializeTheme() {
    const html = document.getElementById("htmlPage");
    const checkbox = document.getElementById("checkbox");

    const savedTheme = localStorage.getItem("theme");
    html.setAttribute("data-bs-theme", savedTheme);
    checkbox.checked = savedTheme === "dark";
    console.log("checkbox.checked", checkbox.checked);
}

function toggleTheme() {
    const html = document.getElementById("htmlPage");
    const checkbox = document.getElementById("checkbox");

    checkbox.addEventListener("change", () => {
        const newTheme = checkbox.checked ? "dark" : "light";
        console.log("newTheme", newTheme);
        html.setAttribute("data-bs-theme", newTheme);
        localStorage.setItem("theme", newTheme);
    });
}

function initializeFontSize() {
    const resizableElements = document.querySelectorAll(".text-resizable");
    const savedFontSize = localStorage.getItem("fontSize");
    const defaultFontSize = 16;

    // 如果有儲存的文字大小，套用到元素
    const fontSize = savedFontSize ? parseInt(savedFontSize, 10) : defaultFontSize;
    resizableElements.forEach(el => {
        el.style.fontSize = fontSize + "px";
    });

    return fontSize; // 回傳目前的文字大小
}

function setupFontResizeButtons() {
    const resizableElements = document.querySelectorAll(".text-resizable");
    let fontSize = initializeFontSize(); // 初始化文字大小

    document.getElementById("increaseFont").addEventListener("click", () => {
        fontSize += 2;
        resizableElements.forEach(el => {
            el.style.fontSize = fontSize + "px";
        });
        localStorage.setItem("fontSize", fontSize);
    });

    document.getElementById("decreaseFont").addEventListener("click", () => {
        fontSize = Math.max(12, fontSize - 2); // 最小文字大小為 12px
        resizableElements.forEach(el => {
            el.style.fontSize = fontSize + "px";
        });
        localStorage.setItem("fontSize", fontSize);
    });

    document.getElementById("resetFont").addEventListener("click", () => {
        fontSize = 16; // 重置為預設文字大小
        resizableElements.forEach(el => {
            el.style.fontSize = fontSize + "px";
        });
        localStorage.setItem("fontSize", fontSize);
    });
}

document.addEventListener("DOMContentLoaded", () => {
    initializeTheme();
    toggleTheme();
    setupFontResizeButtons(); // 設定縮放文字功能
});
