import { Link } from "react-router-dom";
export default function Home() {
    return (
        <div className="container py-5">
            <h1 className="mb-4">WeLovePDF</h1>
            <div className="row g-4">
                <div className="col-md-4">
                    <Link to="/merge" className="card p-4 text-decoration-none shadow-sm">
                        <h5>Merge PDF</h5>
                        <p>Combine multiple PDFs into one</p>
                    </Link>
                </div>
                <div className="col-md-4">
                    <Link to="/split" className="card p-4 text-decoration-none shadow-sm">
                        <h5>Split PDF</h5>
                        <p>Split PDF into individual pages</p>
                    </Link>
                </div>
            </div>
        </div>
    );
}