import { Routes, Route } from 'react-router-dom'
import Layout from './Layout'
import Home from './pages/Home'
import Results from './pages/Results'
import LinePrecision from './pages/LinePrecision'
import Reflex from './pages/Reflex'
import TumorAblation from './pages/TumorAblation'
import Suture from './pages/Suture'
import TumorSpotting from './pages/TumorSpotting'
import SteadyHand from './pages/SteadyHand'
import InstrumentSequence from './pages/InstrumentSequence'

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/results" element={<Results />} />
        <Route path="/line-precision" element={<LinePrecision />} />
        <Route path="/reflex" element={<Reflex />} />
        <Route path="/tumor-ablation" element={<TumorAblation />} />
        <Route path="/suture" element={<Suture />} />
        <Route path="/tumor-spotting" element={<TumorSpotting />} />
        <Route path="/steady-hand" element={<SteadyHand />} />
        <Route path="/instrument-sequence" element={<InstrumentSequence />} />
      </Routes>
    </Layout>
  )
}
