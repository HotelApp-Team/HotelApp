// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Admin filtre dropdownlarini aranabilir, modern Tom Select bilesenine cevirir.
// ".admin-filter-select" sinifi olan tum <select> ogeleri baslatilir.
// AJAX ile icerik degistiginde tekrar cagrilabilmesi icin global fonksiyon olarak tanimli.
// TomSelect kutuphanesi yuklu degilse (ornegin public sayfalar) sessizce atlanir.
window.initAdminFilterSelects = function (root) {
    if (typeof TomSelect === 'undefined') return;
    var scope = root || document;

    scope.querySelectorAll('.admin-filter-select').forEach(function (el) {
        if (el.tomselect) return; // ayni ogeyi iki kez baslatma

        // Sadece data-searchable="true" olan dropdownlarda arama kutusu olsun.
        // Digerleri (Yildiz, Durum, Siralama, Sira) duz acilir listede kalir.
        var searchable = el.getAttribute('data-searchable') === 'true';

        var settings = {
            // Bos secenek ("Tümü"/"Hepsi") gecerli bir secim olarak kalsin.
            allowEmptyOption: true,
            create: false
        };

        if (!searchable) {
            // controlInput: null -> arama input'u render edilmez, sadece liste acilir.
            settings.controlInput = null;
        } else {
            // Liste her zaman ekleme sirasinda kalsin ("Tümü" ilk eklendigi icin en ustte durur).
            settings.sortField = [{ field: '$order', direction: 'asc' }];

            // Kutuya odaklaninca: secim "Tümü" (bos) ise gosterimi temizle ki
            // kutu temiz gorunsun ve kullanici dogrudan yazmaya baslayabilsin.
            // Gercek bir secim varsa (orn. bir sehir) ona dokunmayiz.
            settings.onFocus = function () {
                if (this.items.length && this.getValue() === '') {
                    this.clear(true); // sessiz: change tetiklenmesin
                }
                this.setTextboxValue('');
                this.refreshOptions(false);
            };

            // Kutudan cikildiginda hicbir secim kalmadiysa "Tümü"yu geri yukle.
            settings.onBlur = function () {
                if (this.items.length === 0) {
                    this.setValue('', true);
                }
            };
        }

        new TomSelect(el, settings);
    });
};

document.addEventListener('DOMContentLoaded', function () {
    window.initAdminFilterSelects(document);
});
