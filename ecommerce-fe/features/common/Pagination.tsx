import React from "react";

type PaginationProps = {
	page: any;
	totalPages: any;
	onPageChange: (page: number) => void;
};

const Pagination = ({ page, totalPages, onPageChange }: PaginationProps) => {
	return (
		<div className="flex items-center justify-center gap-2 mt-4">
			<button
				className="px-3 py-1 rounded border text-sm disabled:opacity-50"
				onClick={() => onPageChange(1)}
				disabled={page === 1}
				aria-label="Trang đầu"
			>
				{"<<"}
			</button>
			<button
				className="px-3 py-1 rounded border text-sm disabled:opacity-50"
				onClick={() => onPageChange(page - 1)}
				disabled={page === 1}
				aria-label="Trang trước"
			>
				{"<"}
			</button>
			<span className="px-2 text-sm">
				Trang <b>{page}</b> / {totalPages}
			</span>
			<button
				className="px-3 py-1 rounded border text-sm disabled:opacity-50"
				onClick={() => onPageChange(page + 1)}
				disabled={page === totalPages}
				aria-label="Trang sau"
			>
				{">"}
			</button>
			<button
				className="px-3 py-1 rounded border text-sm disabled:opacity-50"
				onClick={() => onPageChange(totalPages)}
				disabled={page === totalPages}
				aria-label="Trang cuối"
			>
				{">>"}
			</button>
		</div>
	);
};

export default Pagination;
