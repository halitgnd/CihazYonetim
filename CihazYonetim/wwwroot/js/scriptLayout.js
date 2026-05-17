// Her 2 saniyede bir (2000 milisaniye) bu fonksiyon çalışacak
setInterval(function () {
    // 10 ile 100 arasında rastgele sayı üret
    let rastgeleHiz = (Math.random() * (100 - 10) + 10).toFixed(1);

    // id'si 'canli-hiz' olan elementi bul ve içindeki metni değiştir
    document.getElementById('canli-hiz').innerText = rastgeleHiz.replace('.', ',') + " Mbps";
}, 2000);

document.addEventListener("DOMContentLoaded", function () {
    const navItems = document.querySelectorAll(".nav_ul li");

    navItems.forEach(item => {
        // SADECE 'critical' class'ına sahip noktaları arıyoruz!
        // Diğerleri (Offline, Cihaz Sayısı vb.) bu radara takılmayacak.
        const dot = item.querySelector(".status-dot.critical");
        const counter = item.querySelector(".counter");
        
        if (dot && counter) {
            const countValue = parseInt(counter.textContent.trim());
            if (countValue > 0) {
                dot.classList.add("pulse");
            } else {
                dot.classList.remove("pulse");
            }
        }
    });
});

$(document).ready(function() {
    // Layout'taki "Yeni Cihaz Ekle" butonu için tetikleyici
    $(".nav_ul button:first").on("click", function() {
        $("#yeniCihazPaneli").fadeToggle(200);
    });

    $("#cihazEkleForm").on("submit", function(e) {
        e.preventDefault();
        $.post("/Cihaz/Ekle", $(this).serialize(), function(r) {
            if(r.success) location.reload();
        });
    });
});

