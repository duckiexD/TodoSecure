document.addEventListener("DOMContentLoaded", () => {
    const buttons = document.querySelectorAll(".mark-done");
    if (!buttons.length) return;

    const meta = document.querySelector('meta[name="csrf-token"]');
    if (!meta) {
        console.warn("CSRF meta-тег не найден");
        return;
    }
    const csrfToken = meta.getAttribute("content");

    buttons.forEach(btn => {
        btn.addEventListener("click", async (e) => {
            e.preventDefault();

            const id = btn.dataset.id;
            if (!id) return;

            btn.disabled = true;

            try {
                const resp = await fetch(`/Tasks/MarkDone?id=${encodeURIComponent(id)}`, {
                    method: "POST",
                    headers: {
                        "X-CSRF-TOKEN": csrfToken,
                        "Accept": "application/json"
                    },
                    credentials: "same-origin"
                });

                if (resp.ok) {
                    const row = btn.closest("tr");
                    if (row) {
                        const statusCell = row.querySelector("td:nth-child(4)");
                        if (statusCell) statusCell.textContent = "✔";
                    }
                    btn.remove();
                } else if (resp.status === 400) {
                    alert("Ошибка CSRF-токена (400). Обновите страницу.");
                    btn.disabled = false;
                } else if (resp.status === 403) {
                    alert("Нет прав на изменение этой задачи (403).");
                    btn.disabled = false;
                } else if (resp.status === 401) {
                    window.location.href = "/Account/Login";
                } else {
                    alert("Ошибка: " + resp.status);
                    btn.disabled = false;
                }
            } catch (err) {
                console.error(err);
                alert("Сетевая ошибка");
                btn.disabled = false;
            }
        });
    });
});