/**
 * 此模組提供把多張圖片上傳的功能。
 * 使用 Dropzone + Sortable
 * 此處的耦合為:
 * Dropzone 會新增 domElement，呼叫方需要知道結構才能控制樣式 
 * Sortable 需要外層容器 DOM + 定義拖曳元素的 class
 */

/**
 * 初始化包裝過的 Sortable Dropzone
 * @param {Object} domObj - 要當成 dropzone 的元素
 * @param {string} previewTemplate - 新增為 image preview 的元素構造 
 * @returns {Promise<IDBDatabase>}
 */
function init(domObj, previewTemplate) {

    // 禁止 Dropzone 自動掃描頁面上 class。
    // 禁止 Dropzone 自動初始化上傳功能。
    Dropzone.autoDiscover = false;

    // 定義最大上傳圖片張數
    const MAX_FILES = 9;

    // 初始化 Dropzone: 讓圖片可以拖曳進入 image-dropzone
    const sdz = new Dropzone(domObj, {
        // 必要參數，即使是假的。
        url: '/noop',
        // 目前不真實傳送，所以手動控制
        autoProcessQueue: false,
        // 允許上傳的格式
        acceptedFiles: 'image/jpeg,image/png,image/gif,image/webp',
        dictInvalidFileType: '只允許上傳圖片檔案jpg, jpeg, png, gif, webp',
        // 新增的圖片 preview DOM 結構
        previewTemplate,
        // 最大張數
        maxFiles: MAX_FILES, // 軟限制
        dictDefaultMessage: `最多只能上傳 ${MAX_FILES} 張圖片`,
        dictMaxFilesExceeded: `最多只能上傳 ${MAX_FILES} 張圖片`,

        dictDefaultMessage: `拖曳圖片或點擊上傳（最多 ${MAX_FILES} 張）`,

        // "取代預設行為" 而非 "附加額外行為"
        // 延後 DOM 移除 → 加動畫後再刪
        removedfile: function (file) {
            const preview = file.previewElement;
            if (preview) {
                preview.classList.add("animate__animated", "animate__fadeOut");
                preview.addEventListener("animationend", () => {
                    preview.remove(); // 動畫結束後再移除 DOM
                }, { once: true });
            }
        }
    });

    // 初始化 SortableJS: 拖動 dz-preview 來排序
    new Sortable(domObj, {
        animation: 150,
        draggable: '.dz-preview',       // 指定目標可拖動的 class
        handle: '.fa-grip',             // 指定目標拖曳句柄的 class
        ghostClass: 'sortable-ghost',   // 指定隱藏佔位元素 class name
        onEnd: () => {
            console.log('已重新排序');
        }
    });

    // 硬性強制限制
    sdz.on("maxfilesexceeded", function (file) {
        sdz.removeFile(file);
    });

    // 加入 Dropzone 檔案事件。
    sdz.on("addedfile", (file) => {

        // 檢查重複上傳(並未使用 hash 最為依據)
        const dup = sdz.files.find(f =>
            f.name === file.name &&
            f.size === file.size &&
            f !== file // 排除自己
        );
        if (dup) {
            sdz.removeFile(file);
            alert("這張圖片已經加入過了");
            return;
        }

        // 此處根本不送資料，但希望有進度條動畫。
        // 可以用 bootstrap Toasts 推播通知。
        let progress = 0;
        const interval = setInterval(() => {
            progress += 10;

            // 更新 UI
            // progress 是百分比，設定每 50ms 加 10%
            sdz.emit("uploadprogress", file, progress, progress);

            if (progress >= 100) {
                clearInterval(interval);
                sdz.emit("success", file, "模擬成功");
                sdz.emit("complete", file);
            }
        }, 50);
    });

    sdz.on('error', (file, message) => {
        sdz.removeFile(file);
        alert(message);
    });

    return sdz;
}

export default {
    init
};
