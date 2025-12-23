import './index.css'

export default function App() {
  return (
    <div className="min-h-screen bg-gray-900 text-white p-6">
      <h1 className="text-2xl font-bold mb-6">📊 Dashboard</h1>

      <div className="grid grid-cols-3 gap-4">
        <Card title="CPU" value="35%" />
        <Card title="RAM" value="8 GB" />
        <Card title="Status" value="OK" />
      </div>
    </div>
  )
}

function Card({ title, value }: { title: string; value: string }) {
  return (
    <div className="bg-gray-800 rounded-xl p-4 shadow">
      <p className="text-gray-400 text-sm">{title}</p>
      <p className="text-xl font-semibold">{value}</p>
    </div>
  )
}