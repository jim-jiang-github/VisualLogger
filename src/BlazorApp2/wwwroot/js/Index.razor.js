
const thead = document.querySelectorAll('.table thead')[0];
let draggableCols = document.querySelectorAll('.table-draggable th');
let resizableCols = document.querySelectorAll('.table-resizable th');
resizableCols.forEach(function (col) {
    var p = 100 / draggableCols.length;
    col.style.width = `${p}%`;
});

function setDraggable(draggable) {
    draggableCols.forEach(function (col) {
        col.setAttribute('draggable', draggable);
    });
}

function isInDraggableBounds(element, positionX) {
    const bounds = element.getBoundingClientRect();
    const r = bounds.right;
    if (positionX < r - 10) {
        return true;
    } else {
        return false;
    }
}

function isInHeadBounds(positionY) {
    const theadBounds = thead.getBoundingClientRect();
    const t = theadBounds.top;
    const b = theadBounds.bottom;
    if (positionY > t && positionY < b) {
        return true;
    } else {
        return false;
    }
}

export function init() {
    let isResizing;
    let mouseDownX;
    let resizingElement;
    let resizingNextElement;
    let resizingElementWidth;
    let resizingNextElementWidth;
    let isDragging = false;
    let draggingColumnIndex = null;
    let draggedColumnIndex = null;

    function initDrag() {
        setDraggable(true);
        draggableCols.forEach(function (col) {
            col.addEventListener('dragstart', handleDragStart, false);
            col.addEventListener('dragover', handleDragOver, false);
            col.addEventListener('dragleave', handleDragLeave, false);
            col.addEventListener('dragend', handleDragEnd, false);
        });
    }

    function releaseDrag() {
        setDraggable(false);
        draggableCols.forEach(function (col) {
            col.removeEventListener('dragstart', handleDragStart);
            col.removeEventListener('dragover', handleDragOver);
            col.removeEventListener('dragleave', handleDragLeave);
            col.removeEventListener('dragend', handleDragEnd);
        });
    }

    function mouseDownHandler(e) {
        if (!isInHeadBounds(e.clientY)) {
            return;
        }
        if (isInDraggableBounds(e.target, e.clientX)) {
            initDrag();
        } else {
            releaseDrag();
            if (resizableCols.length == 0) {
                return;
            }
            mouseDownX = e.clientX;
            resizingElement = e.target;
            const index = [].slice.call(resizableCols).indexOf(e.target);
            resizingNextElement = resizableCols[index + 1];
            resizingElementWidth = parseFloat(window.getComputedStyle(resizingElement).width);
            resizingNextElementWidth = parseFloat(window.getComputedStyle(resizingNextElement).width);
            document.addEventListener('mouseup', mouseUpHandler, false);
            isResizing = true;
        }
    };

    function mouseMoveHandler(e) {
        if (e.buttons == 1 && isResizing) {
            const offsetX = e.clientX - mouseDownX;
            const theadWidth = parseFloat(window.getComputedStyle(thead).width);
            const resizingElementPercent = resizingElementWidth / theadWidth * 100;
            const resizingNextElementPercent = resizingNextElementWidth / theadWidth * 100;
            let offsetPercent = offsetX / theadWidth * 100;
            //limit min offset = 5%.
            offsetPercent = Math.max(offsetPercent, 3 - resizingElementPercent);
            offsetPercent = Math.min(offsetPercent, resizingNextElementPercent - 3);

            resizingElement.style.width = `${resizingElementPercent + offsetPercent}%`;
            resizingNextElement.style.width = `${resizingNextElementPercent - offsetPercent}%`;
        } else {
            if (isInHeadBounds(e.clientY)) {
                if (isInDraggableBounds(e.target, e.clientX)) {
                    e.target.style.cursor = "grab";
                } else {
                    e.target.style.cursor = "col-resize";
                }
            } else {
                e.target.style.cursor = "default";
            }
        }
    };

    function mouseUpHandler(e) {
        document.removeEventListener('mouseup', mouseUpHandler);
        isResizing = false;
    };

    function handleDragStart(e) {
        this.style.opacity = '0.4';
        draggingColumnIndex = [].slice.call(draggableCols).indexOf(e.target);
    }

    function handleDragOver(e) {
        if (e.preventDefault) {
            e.preventDefault();
        }
        const mouseX = e.clientX;
        const bounds = e.target.getBoundingClientRect();
        const elementCenter = bounds.x + bounds.width / 2;
        draggedColumnIndex = [].slice.call(draggableCols).indexOf(e.target);
        if (draggingColumnIndex == draggedColumnIndex) {
            this.classList.remove('dragging-left');
            this.classList.remove('dragging-right');
            return;
        }
        if (mouseX < elementCenter) {
            this.classList.remove('dragging-right');
            this.classList.add('dragging-left');
        } else {
            this.classList.remove('dragging-left');
            this.classList.add('dragging-right');
            draggedColumnIndex += 1;
        }
    }

    function handleDragLeave(e) {
        this.classList.remove('dragging-left');
        this.classList.remove('dragging-right');
    }

    function handleDragEnd(e) {
        releaseDrag();
        this.style.opacity = '1';
        draggableCols.forEach(function (item) {
            item.classList.remove('dragging-left');
            item.classList.remove('dragging-right');
        });
        if (draggingColumnIndex == draggedColumnIndex) {
            return;
        }

        document.querySelectorAll('.table-draggable tr').forEach(function (row) {
            row.insertBefore(row.children[draggingColumnIndex], row.children[draggedColumnIndex]);
        });
        draggableCols = document.querySelectorAll('.table-draggable th');
        resizableCols = document.querySelectorAll('.table-resizable th');
    }

    document.addEventListener('mousedown', mouseDownHandler, false);
    document.addEventListener('mousemove', mouseMoveHandler, false);
};