$(document).ready(function () {
    let surukleniyorMu = false;

    // === 1. PANELİ AÇIP KAPATAN TETİKÇİ ===
    $(".btn-yeni").on("click", function(e) {
        e.preventDefault();
        let panel = $("#yeniCihazPaneli");
        if (panel.is(":visible")) {
            panel.fadeOut(200);
        } else {
            panel.fadeIn(200).css("display", "flex");
        }
    });

    // === 2. POZİSYON RASTGELE DAĞITICI ===
    let arenaWidth = $(".simulasyon-alani").width();
    let arenaHeight = $(".simulasyon-alani").height();

    $(".cihaz-daire").each(function () {
        if ($(this).css("left") === "0px" || $(this).css("left") === "auto") {
            let rastgeleX = Math.floor(Math.random() * (arenaWidth - 120));
            let rastgeleY = Math.floor(Math.random() * (arenaHeight - 120));
            $(this).css({
                "left": Math.max(10, rastgeleX) + "px",
                "top": Math.max(10, rastgeleY) + "px"
            });
        }
    });

    // === 3. AĞ ÇİZİMİ VE DİNAMİK HIZ MOTORU ===
    setInterval(function() {
        let rastgeleHiz = (Math.random() * (95.0 - 35.0) + 35.0).toFixed(1);
        $("#canli-hiz").text(rastgeleHiz + " Mbps");
        agBaglantilariniCiz();
    }, 15000);

    function agBaglantilariniCiz() {
        const svg = $("#network-cizgileri");
        svg.empty();

        let hizYazisi = $("#canli-hiz").text().replace(/[^0-9.,]/g, '').replace(',', '.');
        let veriHizi = parseFloat(hizYazisi) || 50;
        let animasyonSuresi = Math.max(0.15, 50 / veriHizi);

        let routerlar = $(".cihaz-daire[data-turu='0']");
        let digerCihazlar = $(".cihaz-daire").not("[data-turu='0']");

        if (routerlar.length === 0) return;

        digerCihazlar.each(function () {
            let cihaz = $(this);
            let ucX = cihaz.position().left + (cihaz.width() / 2);
            let ucY = cihaz.position().top + (cihaz.height() / 2);

            let enYakinRouter = null;
            let enKisaMesafe = Infinity;

            routerlar.each(function () {
                let rX = $(this).position().left + ($(this).width() / 2);
                let rY = $(this).position().top + ($(this).height() / 2);
                let dx = rX - ucX;
                let dy = rY - ucY;
                let mesafe = Math.sqrt((dx * dx) + (dy * dy));

                if (mesafe < enKisaMesafe) {
                    enKisaMesafe = mesafe;
                    enYakinRouter = $(this);
                }
            });

            if (enYakinRouter) {
                let merkezX = enYakinRouter.position().left + (enYakinRouter.width() / 2);
                let merkezY = enYakinRouter.position().top + (enYakinRouter.height() / 2);

                let cihazDurum = cihaz.attr("data-durum");
                let routerDurum = enYakinRouter.attr("data-durum");
                let baglantiAktifMi = (cihazDurum == 1 && routerDurum == 1);

                let cizgiRengi = baglantiAktifMi ? "rgba(46, 204, 113, 0.8)" : "rgba(255, 255, 255, 0.1)";
                let kalinlik = baglantiAktifMi ? "3" : "1";

                let line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                line.setAttribute('x1', merkezX);
                line.setAttribute('y1', merkezY);
                line.setAttribute('x2', ucX);
                line.setAttribute('y2', ucY);
                line.setAttribute('stroke', cizgiRengi);
                line.setAttribute('stroke-width', kalinlik);

                if (baglantiAktifMi) {
                    line.setAttribute('stroke-dasharray', '8, 8');
                    line.setAttribute('class', 'cizgi-aktif');
                    line.style.animationDuration = animasyonSuresi + "s";
                }
                svg.append(line);
            }
        });
    }

    agBaglantilariniCiz();

    // === 4. SÜRÜKLE BIRAK SİSTEMİ ===
    $(".cihaz-daire").draggable({
        containment: ".simulasyon-alani",
        scroll: false,
        start: function() {
            surukleniyorMu = true;
            $(this).css("z-index", 1000);
        },
        drag: function() { agBaglantilariniCiz(); },
        stop: function(event, ui) {
            $(this).css("z-index", 100);
            $.post('/Cihaz/PozisyonGuncelle', { cihazId: $(this).attr("data-id"), x: ui.position.left, y: ui.position.top });
            setTimeout(function() { surukleniyorMu = false; }, 100);
        }
    });

    // === 5. NAVBAR SAYAC GÜNCELLEYİCİ ===
    function navbarGuncelle(eskiDurum, yeniDurum) {
        if(eskiDurum === yeniDurum) return;
        const idMap = { 0: "#nav-offline", 1: "#nav-online", 2: "#nav-ariza", 3: "#nav-bakim" };
        let eskiSayacEl = $(idMap[eskiDurum]);
        let yeniSayacEl = $(idMap[yeniDurum]);

        if(eskiSayacEl.length) eskiSayacEl.text(Math.max(0, (parseInt(eskiSayacEl.text()) || 0) - 1));
        if(yeniSayacEl.length) yeniSayacEl.text((parseInt(yeniSayacEl.text()) || 0) + 1);
    }

    // === 6. TIKLAMA VE DURUM DEĞİŞTİRME (KESİN YENİLENMEZ) ===
    $(document).on("click", ".cihaz-icerik, .ariza-btn, .bakim-btn", function (e) {
        e.preventDefault(); // BALYOZ BURADA: Sayfanın yenilenmesini kesin olarak yasaklar.

        if (surukleniyorMu) return; // Sürüklerken tıklamayı engeller

        let top = $(this).closest(".cihaz-daire");

        // GÜVENLİK DUVARI: Yetkisi yoksa anında kes!
        if (top.attr("data-yetki") === "yok") {
            alert("Dikkat: Sadece kendi zimmetindeki cihazlara müdahale edebilirsin!");
            return;
        }

        let cihazId = top.attr("data-id");
        let mevcutDurum = parseInt(top.attr("data-durum"));
        let yeniDurum;

        // Tıklanan yere göre aksiyon belirle
        if ($(this).hasClass("ariza-btn")) {
            yeniDurum = 2; // A butonuna basıldı
        } else if ($(this).hasClass("bakim-btn")) {
            yeniDurum = 3; // B butonuna basıldı
        } else {
            // Merkeze basıldıysa Online <-> Offline geçişi yap
            yeniDurum = (mevcutDurum === 1) ? 0 : 1;
        }

        if (mevcutDurum === yeniDurum) return; // Zaten o durumdaysa yorma sistemi

        // === ASIL OPERASYON BURADA: ARKA PLANDA SESSİZCE HABERLEŞ ===
        $.ajax({
            url: '/Cihaz/DurumGuncelle',
            type: 'POST',
            data: { cihazId: cihazId, yeniDurum: yeniDurum },
            success: function (res) {
                if(res.success) {
                    // Veritabanı onayladı, görseli değiştir!
                    navbarGuncelle(mevcutDurum, yeniDurum);
                    top.attr("data-durum", yeniDurum);

                    if (yeniDurum === 1) {
                        top.css({"background-color": "#198754", "box-shadow": "0 0 20px #198754"});
                        top.find(".daire-merkez").text("Online");
                    } else if (yeniDurum === 0) {
                        top.css({"background-color": "#6c757d", "box-shadow": "none"});
                        top.find(".daire-merkez").text("Offline");
                    } else if (yeniDurum === 2) {
                        top.css({"background-color": "#dc3545", "box-shadow": "0 0 25px #dc3545"});
                        top.find(".daire-merkez").text("Arıza");
                    } else if (yeniDurum === 3) {
                        top.css({"background-color": "#fd7e14", "box-shadow": "0 0 20px #fd7e14"});
                        top.find(".daire-merkez").text("Bakım");
                    }

                    agBaglantilariniCiz(); // Çizgileri yeni duruma göre güncelle
                } else {
                    alert(res.message); // Arka kapıdan yetkisiz girene tokat
                }
            },
            error: function () {
                console.log("AJAX Hatası: Durum güncellenemedi.");
            }
        });
    });
});