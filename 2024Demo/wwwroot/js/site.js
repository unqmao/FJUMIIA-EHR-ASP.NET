function initializeTheme() {
    const html = document.getElementById("htmlPage");
    const checkbox = document.getElementById("checkbox");

    const savedTheme = localStorage.getItem("theme")
    html.setAttribute("data-bs-theme", savedTheme);
    checkbox.checked = savedTheme === "dark";
    console.log("checkbox.checked ", checkbox.checked)
}

function toggleTheme() {
    const html = document.getElementById("htmlPage");
    const checkbox = document.getElementById("checkbox");

    checkbox.addEventListener("change", () => {
        const newTheme = checkbox.checked ? "dark" : "light"
        console.log("newTheme ", newTheme)
        html.setAttribute("data-bs-theme", newTheme);
        localStorage.setItem("theme", newTheme)
    })
}
document.addEventListener("DOMContentLoaded", () => {
    initializeTheme();
    toggleTheme();
});
